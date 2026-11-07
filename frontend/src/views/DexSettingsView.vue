<script setup>
import { ref, computed, onMounted } from 'vue'
import { pokemonDex } from '../data/pokedex'

const disabledSlots = ref([])
const search = ref('')
const selectedStatus = ref('all')

const isLoading = ref(true)
const errorMessage = ref('')

async function loadDisabledSlots() {
  try {
    const response = await fetch(
      'http://localhost:5119/api/dex-preferences/disabled'
    )

    if (!response.ok) {
      throw new Error('Could not load dex preferences')
    }

    disabledSlots.value = await response.json()
  } catch (error) {
    console.error(error)

    errorMessage.value =
      'Could not load your Dex settings.'
  } finally {
    isLoading.value = false
  }
}

onMounted(() => {
  loadDisabledSlots()
})

function sameSlot(slotA, slotB) {
  return (
    slotA.dexNumber === slotB.dexNumber &&
    slotA.pokemon === slotB.name &&
    (slotA.form ?? null) === (slotB.form ?? null) &&
    (slotA.gender ?? null) === (slotB.gender ?? null)
  )
}

function isDisabled(entry) {
  return disabledSlots.value.some(
    (slot) => sameSlot(slot, entry)
  )
}

async function toggleSlot(entry) {
  const currentlyDisabled = isDisabled(entry)

  const body = {
    dexNumber: entry.dexNumber,
    pokemon: entry.name,
    form: entry.form ?? null,
    gender: entry.gender ?? null
  }

  try {
    if (currentlyDisabled) {
      const response = await fetch(
        'http://localhost:5119/api/dex-preferences/enable',
        {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json'
          },
          body: JSON.stringify(body)
        }
      )

      if (!response.ok) {
        throw new Error('Could not enable slot')
      }

      disabledSlots.value =
        disabledSlots.value.filter(
          (slot) => !sameSlot(slot, entry)
        )
    } else {
      const response = await fetch(
        'http://localhost:5119/api/dex-preferences/disable',
        {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json'
          },
          body: JSON.stringify(body)
        }
      )

      if (!response.ok) {
        throw new Error('Could not disable slot')
      }

      const savedSlot = await response.json()

      disabledSlots.value.push(savedSlot)
    }
  } catch (error) {
    console.error(error)

    alert('Something went wrong while saving this Dex setting.')
  }
}

async function enableAll() {
  try {
    const response = await fetch(
      'http://localhost:5119/api/dex-preferences/enable-all',
      {
        method: 'DELETE'
      }
    )

    if (!response.ok) {
      throw new Error('Could not enable all slots')
    }

    disabledSlots.value = []
  } catch (error) {
    console.error(error)

    alert('Something went wrong while enabling all Dex slots.')
  }
}

const activeCount = computed(() => {
  return pokemonDex.length - disabledSlots.value.length
})

const filteredEntries = computed(() => {
  const searchValue =
    search.value.trim().toLowerCase()

  return pokemonDex.filter((entry) => {
    const searchableText = [
      entry.name,
      entry.form,
      entry.gender,
      entry.dexNumber
    ]
      .filter(Boolean)
      .join(' ')
      .toLowerCase()

    if (
      searchValue &&
      !searchableText.includes(searchValue)
    ) {
      return false
    }

    const disabled = isDisabled(entry)

    if (
      selectedStatus.value === 'active' &&
      disabled
    ) {
      return false
    }

    if (
      selectedStatus.value === 'disabled' &&
      !disabled
    ) {
      return false
    }

    return true
  })
})

function getEntryLabel(entry) {
  const details = []

  if (entry.form) {
    details.push(entry.form)
  }

  if (entry.gender) {
    details.push(entry.gender)
  }

  if (details.length === 0) {
    return 'Default'
  }

  return details.join(' • ')
}
</script>

<template>
  <main class="settings-page">
    <section class="page-header">
      <h1>Customize Dex</h1>

      <p>
        Choose which Pokémon and forms count toward your
        personal Shiny Living Dex.
      </p>

      <div class="dex-summary">
        <strong>
          {{ activeCount }} / {{ pokemonDex.length }}
        </strong>

        <span>
          Dex slots active
        </span>
      </div>
    </section>

    <p
      v-if="isLoading"
      class="message"
    >
      Loading Dex settings...
    </p>

    <p
      v-else-if="errorMessage"
      class="message error"
    >
      {{ errorMessage }}
    </p>

    <template v-else>
      <section class="controls">
        <input
          v-model="search"
          type="text"
          class="search"
          placeholder="Search Pokémon or form..."
        />

        <select
          v-model="selectedStatus"
          class="status-filter"
        >
          <option value="all">
            All Slots
          </option>

          <option value="active">
            Active
          </option>

          <option value="disabled">
            Disabled
          </option>
        </select>

        <button
          type="button"
          class="enable-all-button"
          :disabled="disabledSlots.length === 0"
          @click="enableAll"
        >
          Enable All
        </button>
      </section>

      <section class="dex-list">
        <div
          v-for="entry in filteredEntries"
          :key="`${entry.dexNumber}-${entry.name}-${entry.form ?? 'default'}-${entry.gender ?? 'default'}`"
          class="dex-row"
          :class="{
            disabled: isDisabled(entry)
          }"
        >
          <div class="entry-info">
            <span class="dex-number">
              #{{ String(entry.dexNumber).padStart(4, '0') }}
            </span>

            <div>
              <strong class="pokemon-name">
                {{ entry.name }}
              </strong>

              <span class="entry-details">
                {{ getEntryLabel(entry) }}
              </span>
            </div>
          </div>

          <button
            type="button"
            class="toggle-button"
            :class="{
              inactive: isDisabled(entry)
            }"
            @click="toggleSlot(entry)"
          >
            {{ isDisabled(entry) ? 'Disabled' : 'Active' }}
          </button>
        </div>
      </section>

      <p
        v-if="filteredEntries.length === 0"
        class="no-results"
      >
        No Dex slots found.
      </p>
    </template>
  </main>
</template>

<style scoped>
.settings-page {
  max-width: 1000px;
  margin: 0 auto;
  padding: 50px 20px;
}

.page-header {
  text-align: center;
  margin-bottom: 32px;
}

.page-header h1 {
  margin: 0;
  font-size: 2.4rem;
}

.page-header p {
  margin: 10px auto 0;
  max-width: 600px;
  color: #6b7280;
}

.dex-summary {
  margin-top: 20px;
}

.dex-summary strong {
  display: block;
  font-size: 1.5rem;
}

.dex-summary span {
  color: #6b7280;
}

.message {
  text-align: center;
  font-weight: 600;
}

.error {
  color: #b91c1c;
}

.controls {
  display: grid;
  grid-template-columns: 1fr 180px auto;
  gap: 12px;

  margin-bottom: 24px;
}

.search,
.status-filter {
  padding: 12px 14px;

  border: 1px solid #d1d5db;
  border-radius: 10px;

  background: white;
}

.enable-all-button {
  padding: 12px 18px;

  border: none;
  border-radius: 10px;

  background: #111827;
  color: white;

  font-weight: 600;
  cursor: pointer;
}

.enable-all-button:disabled {
  opacity: 0.4;
  cursor: default;
}

.dex-list {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.dex-row {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 20px;

  padding: 14px 18px;

  border: 1px solid #e5e7eb;
  border-radius: 14px;

  background: white;

  transition:
    opacity 0.2s ease,
    background 0.2s ease;
}

.dex-row.disabled {
  opacity: 0.6;
  background: #f9fafb;
}

.entry-info {
  display: flex;
  align-items: center;
  gap: 16px;
}

.dex-number {
  min-width: 55px;

  color: #6b7280;
  font-size: 0.9rem;
  font-weight: 600;
}

.pokemon-name {
  display: block;
}

.entry-details {
  display: block;
  margin-top: 3px;

  color: #6b7280;
  font-size: 0.85rem;
}

.toggle-button {
  min-width: 90px;
  padding: 9px 14px;

  border: none;
  border-radius: 999px;

  background: #dcfce7;
  color: #166534;

  font-weight: 700;
  cursor: pointer;
}

.toggle-button.inactive {
  background: #fee2e2;
  color: #991b1b;
}

.no-results {
  margin-top: 30px;
  text-align: center;
  color: #6b7280;
}

@media (max-width: 650px) {
  .settings-page {
    padding: 30px 16px;
  }

  .page-header h1 {
    font-size: 2rem;
  }

  .controls {
    grid-template-columns: 1fr;
  }

  .dex-row {
    padding: 12px;
  }

  .entry-info {
    gap: 10px;
  }

  .dex-number {
    min-width: 48px;
  }
}
</style>