<script setup>
import { ref, computed, onMounted, watch } from 'vue'
import { pokemonDex } from '../data/pokedex'
import DexCard from '../components/DexCard.vue'

const shinies = ref([])
const search = ref('')
const currentPage = ref(1)
const itemsPerPage = 60

async function loadShinies() {
  const response = await fetch('http://localhost:5119/api/shinies')
  const data = await response.json()

  shinies.value = data
}

onMounted(() => {
  loadShinies()
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

const filteredDexEntries = computed(() => {
  return dexEntries.value.filter((entry) =>
    entry.name.toLowerCase().includes(search.value.toLowerCase())
  )
})

const totalPages = computed(() => {
  return Math.ceil(filteredDexEntries.value.length / itemsPerPage)
})

const paginatedDexEntries = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage
  const end = start + itemsPerPage

  return filteredDexEntries.value.slice(start, end)
})

const obtainedCount = computed(() => {
  return dexEntries.value.filter((entry) => entry.obtained).length
})

const progressPercentage = computed(() => {
  if (dexEntries.value.length === 0) {
    return 0
  }

  return Math.round(
    (obtainedCount.value / dexEntries.value.length) * 100
  )
})

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
</script>

<template>
  <main class="page">
    <section class="hero">
      <h1>My Shiny Collection</h1>

      <section class="progress-section">
        <div class="progress-info">
          <span>Living Dex Progress</span>

          <span>
            {{ obtainedCount }} / {{ dexEntries.length }}
            — {{ progressPercentage }}%
          </span>
        </div>

        <div class="progress-bar">
          <div
            class="progress-fill"
            :style="{ width: `${progressPercentage}%` }"
          ></div>
        </div>
      </section>

      <input
        class="search"
        type="text"
        v-model="search"
        placeholder="Search Pokémon..."
      />
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

.search {
  width: 100%;
  max-width: 400px;
  padding: 12px 16px;
  margin-top: 16px;
}

.grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 28px;
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
</style>