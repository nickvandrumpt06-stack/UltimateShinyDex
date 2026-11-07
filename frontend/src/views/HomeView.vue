<script setup>
import { ref, computed, onMounted, watch } from 'vue'
import { useRoute } from 'vue-router'
import { pokemonDex } from '../data/pokedex'
import { games } from '../data/options'
import { gameGroups } from '../data/gameGroups'
import DexCard from '../components/DexCard.vue'

const route = useRoute()

const shinies = ref([])
const disabledSlots = ref([])
const search = ref('')
const currentPage = ref(1)

const selectedGame = ref('')
const selectedStatus = ref('')
const selectedAlpha = ref('')
const selectedMarks = ref('')
const selectedGeneration = ref('')

const showFilters = ref(false)

const itemsPerPage = 60

const activeDexEntries = computed(() => {
  return dexEntries.value.filter(
    (entry) => !isDexSlotDisabled(entry)
  )
})

watch(currentPage, () => {
  window.scrollTo({
    top: 0,
    behavior: 'smooth'
  })
})

async function loadShinies() {
  const response = await fetch('http://localhost:5119/api/shinies')
  const data = await response.json()

  shinies.value = data
}

onMounted(() => {
  loadShinies()
  loadDisabledSlots()
})

const selectedGameGroup = computed(() => {
  const groupKey = route.query.gameGroup

  if (!groupKey) {
    return null
  }

  return gameGroups.find(
    (group) => group.key === groupKey
  ) ?? null
})

const dexEntries = computed(() => {
  return pokemonDex.map((pokemon) => {
    const ownedShinies = shinies.value.filter((shiny) => {
      const samePokemon =
        shiny.pokemon.toLowerCase() === pokemon.name.toLowerCase()

      if (!samePokemon) {
        return false
      }

      const sameGender =
        !pokemon.gender || shiny.gender === pokemon.gender

      const sameForm =
        !pokemon.form
          ? !shiny.form
          : shiny.form === pokemon.form

      return sameGender && sameForm
    })

    return {
      ...pokemon,
      obtained: ownedShinies.length > 0,
      shinies: ownedShinies
    }
  })
})

function getGeneration(dexNumber) {
  if (dexNumber <= 151) return 1
  if (dexNumber <= 251) return 2
  if (dexNumber <= 386) return 3
  if (dexNumber <= 493) return 4
  if (dexNumber <= 649) return 5
  if (dexNumber <= 721) return 6
  if (dexNumber <= 809) return 7
  if (dexNumber <= 905) return 8

  return 9
}

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

const filteredDexEntries = computed(() => {
  return activeDexEntries.value
  .map((entry) => {
      const matchesSearch =
        entry.name
          .toLowerCase()
          .includes(search.value.toLowerCase())

      if (!matchesSearch) {
        return null
      }

      if (
        selectedGeneration.value &&
        getGeneration(entry.dexNumber) !==
          Number(selectedGeneration.value)
      ) {
        return null
      }

      if (
        selectedStatus.value === 'obtained' &&
        !entry.obtained
      ) {
        return null
      }

      if (
        selectedStatus.value === 'missing' &&
        entry.obtained
      ) {
        return null
      }

      let filteredShinies = entry.shinies

      if (selectedGameGroup.value) {
        filteredShinies = filteredShinies.filter(
          (shiny) =>
            selectedGameGroup.value.games.includes(shiny.game)
        )
      } else if (selectedGame.value) {
        filteredShinies = filteredShinies.filter(
          (shiny) => shiny.game === selectedGame.value
        )
      }

      if (selectedAlpha.value === 'alpha') {
        filteredShinies = filteredShinies.filter(
          (shiny) => shiny.isAlpha
        )
      }

      if (selectedAlpha.value === 'non-alpha') {
        filteredShinies = filteredShinies.filter(
          (shiny) => !shiny.isAlpha
        )
      }

      if (selectedMarks.value === 'marked') {
        filteredShinies = filteredShinies.filter(
          (shiny) => shiny.marks?.length > 0
        )
      }

      if (selectedMarks.value === 'unmarked') {
        filteredShinies = filteredShinies.filter(
          (shiny) => !shiny.marks?.length
        )
      }

      const shinyFilterActive =
        selectedGameGroup.value ||
        selectedGame.value ||
        selectedAlpha.value ||
        selectedMarks.value

      if (
        shinyFilterActive &&
        filteredShinies.length === 0
      ) {
        return null
      }

      return {
        ...entry,
        shinies: filteredShinies
      }
    })
    .filter((entry) => entry !== null)
})

const totalPages = computed(() => {
  return Math.ceil(
    filteredDexEntries.value.length / itemsPerPage
  )
})

const paginatedDexEntries = computed(() => {
  const start =
    (currentPage.value - 1) * itemsPerPage

  const end = start + itemsPerPage

  return filteredDexEntries.value.slice(start, end)
})

const obtainedCount = computed(() => {
  return activeDexEntries.value.filter(
    (entry) => entry.obtained
  ).length
})

const totalShinyCount = computed(() => {
  return shinies.value.length
})

const progressPercentage = computed(() => {
  if (activeDexEntries.value.length === 0) {
    return 0
  }

  return Math.round(
    (
      obtainedCount.value /
      activeDexEntries.value.length
    ) * 100
  )
})

const activeFilterCount = computed(() => {
  let count = 0

  if (selectedGameGroup.value || selectedGame.value) {
    count++
  }

  if (selectedStatus.value) {
    count++
  }

  if (selectedAlpha.value) {
    count++
  }

  if (selectedMarks.value) {
    count++
  }

  if (selectedGeneration.value) {
    count++
  }

  return count
})

function clearFilters() {
  selectedGame.value = ''
  selectedStatus.value = ''
  selectedAlpha.value = ''
  selectedMarks.value = ''
  selectedGeneration.value = ''
  search.value = ''
  currentPage.value = 1
}

async function loadDisabledSlots() {
  const response = await fetch(
    'http://localhost:5119/api/dex-preferences/disabled'
  )

  disabledSlots.value = await response.json()
}

function previousPage() {
  if (currentPage.value > 1) {
    currentPage.value--
  }
}

function nextPage() {
  if (currentPage.value < totalPages.value) {
    currentPage.value++
  }
}

watch(search, () => {
  currentPage.value = 1
})

watch(selectedGame, () => {
  currentPage.value = 1
})

watch(
  () => route.query.gameGroup,
  () => {
    currentPage.value = 1
  }
)

watch(
  [
    selectedStatus,
    selectedAlpha,
    selectedMarks,
    selectedGeneration
  ],
  () => {
    currentPage.value = 1
  }
)
</script>

<template>
  <main class="page">
    <section class="hero">
      <h1>My Shiny Collection</h1>

      <section class="progress-section">
        <div class="progress-info">
          <span>Living Dex Progress</span>

          <span>
            {{ obtainedCount }} / {{ activeDexEntries.length }}
— {{ progressPercentage }}%
          </span>
        </div>

        <div class="progress-bar">
          <div
            class="progress-fill"
            :style="{ width: `${progressPercentage}%` }"
          ></div>
        </div>

        <div class="total-shinies">
          Total Shinies: {{ totalShinyCount }}
        </div>
      </section>

      <div class="search-filter-row">
        <input
          class="search"
          type="text"
          v-model="search"
          placeholder="Search Pokémon..."
        />

        <button
          class="filter-toggle"
          type="button"
          @click="showFilters = !showFilters"
        >
          Filters
          <span v-if="activeFilterCount > 0">
            ({{ activeFilterCount }})
          </span>
        </button>
      </div>

      <p
        v-if="selectedGameGroup"
        class="active-filter"
      >
        Showing: {{ selectedGameGroup.name }}
      </p>

      <div
        v-if="showFilters"
        class="filter-panel"
      >
        <div class="filter-group">
          <label for="gameFilter">
            Game
          </label>

          <select
            id="gameFilter"
            class="filter-select"
            v-model="selectedGame"
            :disabled="Boolean(selectedGameGroup)"
          >
            <option value="">
              All Games
            </option>

            <option
              v-for="gameOption in games"
              :key="gameOption"
              :value="gameOption"
            >
              {{ gameOption }}
            </option>
          </select>
        </div>

        <div class="filter-group">
          <label for="statusFilter">
            Status
          </label>

          <select
            id="statusFilter"
            class="filter-select"
            v-model="selectedStatus"
          >
            <option value="">
              All Statuses
            </option>

            <option value="obtained">
              Obtained
            </option>

            <option value="missing">
              Missing
            </option>
          </select>
        </div>

        <div class="filter-group">
          <label for="alphaFilter">
            Alpha
          </label>

          <select
            id="alphaFilter"
            class="filter-select"
            v-model="selectedAlpha"
          >
            <option value="">
              All Alpha Statuses
            </option>

            <option value="alpha">
              Alpha
            </option>

            <option value="non-alpha">
              Non-Alpha
            </option>
          </select>
        </div>

        <div class="filter-group">
          <label for="marksFilter">
            Marks & Ribbons
          </label>

          <select
            id="marksFilter"
            class="filter-select"
            v-model="selectedMarks"
          >
            <option value="">
              All Mark Statuses
            </option>

            <option value="marked">
              Has Mark / Ribbon
            </option>

            <option value="unmarked">
              No Marks / Ribbons
            </option>
          </select>
        </div>

        <div class="filter-group">
          <label for="generationFilter">
            Generation
          </label>

          <select
            id="generationFilter"
            class="filter-select"
            v-model="selectedGeneration"
          >
            <option value="">
              All Generations
            </option>

            <option
              v-for="generation in 9"
              :key="generation"
              :value="generation"
            >
              Generation {{ generation }}
            </option>
          </select>
        </div>

        <button
          class="clear-filters"
          type="button"
          @click="clearFilters"
        >
          Clear Filters
        </button>
      </div>
    </section>

    <section class="grid">
      <DexCard
        v-for="entry in paginatedDexEntries"
        :key="`${entry.dexNumber}-${entry.form ?? 'normal'}-${entry.gender ?? 'default'}`"
        :entry="entry"
      />
    </section>

    <div
      v-if="totalPages > 1"
      class="pagination"
    >
      <button
        @click="previousPage"
        :disabled="currentPage === 1"
      >
        ← Previous
      </button>

      <span>
        Page {{ currentPage }} of {{ totalPages }}
      </span>

      <button
        @click="nextPage"
        :disabled="currentPage === totalPages"
      >
        Next →
      </button>
    </div>
  </main>
</template>

<style scoped>
.page {
  max-width: 1200px;
  margin: 0 auto;
  padding: 40px 20px;
}

.hero {
  text-align: center;
  margin-bottom: 40px;
}

.progress-section {
  max-width: 700px;
  margin: 0 auto 30px;
}

.progress-info {
  display: flex;
  justify-content: space-between;
  margin-bottom: 8px;
  font-weight: 600;
}

.progress-bar {
  width: 100%;
  height: 20px;
  background: #e5e7eb;
  border-radius: 999px;
  overflow: hidden;
}

.progress-fill {
  height: 100%;
  background: #22c55e;
  border-radius: 999px;
  transition: width 0.3s ease;
}

.total-shinies {
  margin-top: 10px;
  font-weight: 600;
}

.search-filter-row {
  display: flex;
  justify-content: center;
  gap: 12px;
  max-width: 700px;
  margin: 16px auto 0;
}

.search {
  flex: 1;
  min-width: 0;
  padding: 12px 16px;
  border: 1px solid #d1d5db;
  border-radius: 10px;
  box-sizing: border-box;
  font-size: 0.95rem;
}

.filter-toggle {
  padding: 12px 20px;
  border: none;
  border-radius: 10px;
  background: #111827;
  color: white;
  font-weight: 600;
  cursor: pointer;
  white-space: nowrap;
}

.filter-toggle:hover {
  opacity: 0.9;
}

.active-filter {
  margin: 12px 0 0;
  font-weight: 600;
  color: #374151;
}

.filter-panel {
  max-width: 700px;
  margin: 16px auto 0;
  padding: 18px;

  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 14px;

  border: 1px solid #e5e7eb;
  border-radius: 16px;
  background: #f9fafb;
}

.filter-group {
  display: flex;
  flex-direction: column;
  gap: 6px;
  text-align: left;
}

.filter-group label {
  font-size: 0.85rem;
  font-weight: 600;
  color: #374151;
}

.filter-select {
  width: 100%;
  padding: 12px 14px;
  border: 1px solid #d1d5db;
  border-radius: 10px;
  background: white;
  box-sizing: border-box;
}

.filter-select:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.clear-filters {
  grid-column: 1 / -1;
  justify-self: center;

  padding: 10px 18px;
  border: none;
  border-radius: 10px;

  background: #e5e7eb;
  color: #111827;

  font-weight: 600;
  cursor: pointer;
}

.clear-filters:hover {
  background: #d1d5db;
}

.grid {
  display: grid;
  grid-template-columns:
    repeat(auto-fit, minmax(250px, 1fr));
  gap: 28px;
}

.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 20px;
  margin-top: 40px;
}

.pagination button {
  padding: 10px 16px;
  border: none;
  border-radius: 10px;
  cursor: pointer;
}

.pagination button:disabled {
  opacity: 0.4;
  cursor: default;
}

.pagination span {
  font-weight: 600;
}

@media (max-width: 650px) {
  .search-filter-row {
    flex-direction: column;
  }

  .filter-panel {
    grid-template-columns: 1fr;
  }

  .clear-filters {
    grid-column: auto;
  }
}
</style>