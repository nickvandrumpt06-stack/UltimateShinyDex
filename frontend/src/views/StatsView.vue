<script setup>
import { ref, computed, onMounted } from 'vue'
import { RouterLink } from 'vue-router'
import { pokemonDex } from '../data/pokedex'

const shinies = ref([])
const disabledSlots = ref([])
const isLoading = ref(true)
const errorMessage = ref('')

async function loadShinies() {
  try {
    const response = await fetch(
      'http://localhost:5119/api/shinies'
    )

    if (!response.ok) {
      throw new Error('Loading shinies failed')
    }

    shinies.value = await response.json()
  } catch (error) {
    console.error(error)

    errorMessage.value =
      'Could not load collection statistics.'
  }
}

async function loadDisabledSlots() {
  try {
    const response = await fetch(
      'http://localhost:5119/api/dex-preferences/disabled'
    )

    if (!response.ok) {
      throw new Error('Loading Dex preferences failed')
    }

    disabledSlots.value = await response.json()
  } catch (error) {
    console.error(error)

    errorMessage.value =
      'Could not load collection statistics.'
  }
}

async function loadStatistics() {
  isLoading.value = true

  await Promise.all([
    loadShinies(),
    loadDisabledSlots()
  ])

  isLoading.value = false
}

onMounted(() => {
  loadStatistics()
})

function isDexSlotDisabled(entry) {
  return disabledSlots.value.some((slot) => {
    return (
      slot.dexNumber === entry.dexNumber &&
      slot.pokemon === entry.name &&
      (slot.form ?? null) === (entry.form ?? null) &&
      (slot.gender ?? null) === (entry.gender ?? null)
    )
  })
}

const totalShinies = computed(() => {
  return shinies.value.length
})

const activePokemonDex = computed(() => {
  return pokemonDex.filter(
    (entry) => !isDexSlotDisabled(entry)
  )
})

const obtainedDexSlots = computed(() => {
  return activePokemonDex.value.filter((pokemon) => {
    return shinies.value.some((shiny) => {
      const samePokemon =
        shiny.pokemon.toLowerCase() ===
        pokemon.name.toLowerCase()

      if (!samePokemon) {
        return false
      }

      const sameGender =
        !pokemon.gender ||
        shiny.gender === pokemon.gender

      const sameForm =
        !pokemon.form
          ? !shiny.form
          : shiny.form === pokemon.form

      return sameGender && sameForm
    })
  }).length
})

const obtainedAllDexSlots = computed(() => {
  return pokemonDex.filter((pokemon) => {
    return shinies.value.some((shiny) => {
      const samePokemon =
        shiny.pokemon.toLowerCase() ===
        pokemon.name.toLowerCase()

      if (!samePokemon) {
        return false
      }

      const sameGender =
        !pokemon.gender ||
        shiny.gender === pokemon.gender

      const sameForm =
        !pokemon.form
          ? !shiny.form
          : shiny.form === pokemon.form

      return sameGender && sameForm
    })
  }).length
})

const completionPercentage = computed(() => {
  if (activePokemonDex.value.length === 0) {
    return 0
  }

  return Math.round(
    (
      obtainedDexSlots.value /
      activePokemonDex.value.length
    ) * 100
  )
})

const duplicateCount = computed(() => {
  return Math.max(
    totalShinies.value - obtainedAllDexSlots.value,
    0
  )
})

const alphaCount = computed(() => {
  return shinies.value.filter(
    (shiny) => shiny.isAlpha
  ).length
})

const markedCount = computed(() => {
  return shinies.value.filter(
    (shiny) => shiny.marks?.length > 0
  ).length
})

const totalEncounters = computed(() => {
  return shinies.value.reduce((total, shiny) => {
    return total + (shiny.encounters ?? 0)
  }, 0)
})

const mostOwnedNature = computed(() => {
  const counts = {}

  for (const shiny of shinies.value) {
    if (!shiny.nature) {
      continue
    }

    counts[shiny.nature] =
      (counts[shiny.nature] ?? 0) + 1
  }

  const ranking = Object.entries(counts)
    .sort((a, b) => b[1] - a[1])

  if (ranking.length === 0) {
    return null
  }

  return {
    name: ranking[0][0],
    count: ranking[0][1]
  }
})

const mostUsedBall = computed(() => {
  const counts = {}

  for (const shiny of shinies.value) {
    if (!shiny.ball) {
      continue
    }

    counts[shiny.ball] =
      (counts[shiny.ball] ?? 0) + 1
  }

  const ranking = Object.entries(counts)
    .sort((a, b) => b[1] - a[1])

  if (ranking.length === 0) {
    return null
  }

  return {
    name: ranking[0][0],
    count: ranking[0][1]
  }
})

const genderBreakdown = computed(() => {
  const counts = {
    Male: 0,
    Female: 0,
    Genderless: 0,
    Unknown: 0
  }

  for (const shiny of shinies.value) {
    if (shiny.gender === 'Male') {
      counts.Male++
    } else if (shiny.gender === 'Female') {
      counts.Female++
    } else if (shiny.gender === 'Genderless') {
      counts.Genderless++
    } else {
      counts.Unknown++
    }
  }

  return counts
})
</script>

<template>
  <main class="stats-page">
    <section class="stats-header">
      <h1>Collection Statistics</h1>

      <p>
        A look at the numbers behind your Shiny Living Dex.
      </p>
    </section>

    <p
      v-if="isLoading"
      class="message"
    >
      Loading statistics...
    </p>

    <p
      v-else-if="errorMessage"
      class="message error"
    >
      {{ errorMessage }}
    </p>

    <template v-else>
      <section class="stat-grid">
        <article class="stat-card">
          <span class="stat-label">
            Total Shinies
          </span>

          <strong class="stat-number">
            {{ totalShinies }}
          </strong>
        </article>

        <article class="stat-card">
          <span class="stat-label">
            Living Dex
          </span>

          <strong class="stat-number">
            {{ obtainedDexSlots }} / {{ activePokemonDex.length }}
          </strong>
        </article>

        <article class="stat-card">
          <span class="stat-label">
            Dex Completion
          </span>

          <strong class="stat-number">
            {{ completionPercentage }}%
          </strong>
        </article>

        <article class="stat-card">
          <span class="stat-label">
            Extra / Duplicate Shinies
          </span>

          <strong class="stat-number">
            {{ duplicateCount }}
          </strong>
        </article>

        <article class="stat-card">
          <span class="stat-label">
            Alpha Shinies
          </span>

          <strong class="stat-number">
            {{ alphaCount }}
          </strong>
        </article>

        <article class="stat-card">
          <span class="stat-label">
            Marked / Ribboned
          </span>

          <strong class="stat-number">
            {{ markedCount }}
          </strong>
        </article>

        <article class="stat-card">
          <span class="stat-label">
            Recorded Encounters
          </span>

          <strong class="stat-number">
            {{ totalEncounters.toLocaleString() }}
          </strong>
        </article>

        <RouterLink
          to="/stats/most-owned"
          class="stat-card stat-link"
        >
          <span class="stat-label">
            Most Owned Pokémon
          </span>

          <strong class="stat-number stat-link-text">
            View Ranking →
          </strong>
        </RouterLink>

        <RouterLink
          to="/stats/games"
          class="stat-card stat-link"
        >
          <span class="stat-label">
            Shinies by Game
          </span>

          <strong class="stat-number stat-link-text">
            View Games →
          </strong>
        </RouterLink>

        <RouterLink
          to="/stats/natures"
          class="stat-card stat-link"
        >
          <span class="stat-label">
            Most Common Nature
          </span>

          <strong
            v-if="mostOwnedNature"
            class="stat-number stat-name"
          >
            {{ mostOwnedNature.name }}
          </strong>

          <span
            v-if="mostOwnedNature"
            class="stat-subtext"
          >
            {{ mostOwnedNature.count }} shinies
          </span>

          <span class="view-details">
            View Nature Ranking →
          </span>
        </RouterLink>

        <RouterLink
          to="/stats/balls"
          class="stat-card stat-link"
        >
          <span class="stat-label">
            Most Used Ball
          </span>

          <strong
            v-if="mostUsedBall"
            class="stat-number stat-name"
          >
            {{ mostUsedBall.name }}
          </strong>

          <span
            v-if="mostUsedBall"
            class="stat-subtext"
          >
            {{ mostUsedBall.count }} shinies
          </span>

          <span class="view-details">
            View Ball Ranking →
          </span>
        </RouterLink>

        <article class="stat-card gender-card">
          <span class="stat-label">
            Gender Breakdown
          </span>

          <div class="gender-list">
            <div class="gender-row">
              <span>Male</span>

              <strong>
                {{ genderBreakdown.Male }}
              </strong>
            </div>

            <div class="gender-row">
              <span>Female</span>

              <strong>
                {{ genderBreakdown.Female }}
              </strong>
            </div>

            <div class="gender-row">
              <span>Genderless</span>

              <strong>
                {{ genderBreakdown.Genderless }}
              </strong>
            </div>

            <div
              v-if="genderBreakdown.Unknown > 0"
              class="gender-row"
            >
              <span>Unknown</span>

              <strong>
                {{ genderBreakdown.Unknown }}
              </strong>
            </div>
          </div>
        </article>
      </section>
    </template>
  </main>
</template>

<style scoped>
.stats-page {
  max-width: 1100px;
  margin: 0 auto;
  padding: 50px 20px;
}

.stats-header {
  text-align: center;
  margin-bottom: 40px;
}

.stats-header h1 {
  margin: 0;
  font-size: 2.4rem;
}

.stats-header p {
  margin-top: 10px;
  color: #6b7280;
}

.message {
  text-align: center;
  font-weight: 600;
}

.error {
  color: #b91c1c;
}

.stat-grid {
  display: grid;
  grid-template-columns:
    repeat(auto-fit, minmax(200px, 1fr));
  gap: 20px;
}

.stat-card {
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 18px;
  padding: 24px;
  text-align: center;
  box-shadow: 0 6px 18px rgba(0, 0, 0, 0.07);
}

.stat-label {
  display: block;
  margin-bottom: 10px;
  color: #6b7280;
  font-weight: 600;
}

.stat-number {
  display: block;
  font-size: 1.8rem;
  color: #111827;
}

.stat-name {
  font-size: 1.5rem;
}

.stat-subtext {
  display: block;
  margin-top: 6px;
  color: #6b7280;
  font-size: 0.9rem;
}

.stat-link {
  text-decoration: none;
  color: inherit;
  cursor: pointer;
  transition:
    transform 0.2s ease,
    box-shadow 0.2s ease;
}

.stat-link:hover {
  transform: translateY(-3px);
  box-shadow: 0 10px 24px rgba(0, 0, 0, 0.12);
}

.stat-link-text {
  font-size: 1.4rem;
}

.view-details {
  display: block;
  margin-top: 12px;
  color: #6b7280;
  font-size: 0.8rem;
  font-weight: 600;
}

.gender-card {
  display: flex;
  flex-direction: column;
}

.gender-list {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.gender-row {
  display: flex;
  justify-content: space-between;
  align-items: center;

  padding: 7px 10px;

  border-radius: 8px;
  background: #f9fafb;
}

.gender-row span {
  color: #374151;
}

.gender-row strong {
  color: #111827;
}

@media (max-width: 600px) {
  .stats-page {
    padding: 30px 16px;
  }

  .stats-header h1 {
    font-size: 2rem;
  }

  .stat-grid {
    grid-template-columns: 1fr 1fr;
  }

  .stat-card {
    padding: 18px 12px;
  }

  .stat-number {
    font-size: 1.4rem;
  }

  .stat-name {
    font-size: 1.2rem;
  }

  .stat-link-text {
    font-size: 1.1rem;
  }

  .gender-row {
    padding: 6px 8px;
    font-size: 0.85rem;
  }
}
</style>