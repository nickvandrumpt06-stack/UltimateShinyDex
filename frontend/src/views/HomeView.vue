<script setup>
import { ref, computed, onMounted } from 'vue'
import ShinyCard from '../components/ShinyCard.vue'
import { pokemonDex } from '../data/pokedex'
import DexCard from '../components/DexCard.vue'

const shinies = ref([])
const search = ref('')

const filteredDexEntries = computed(() => {
  return dexEntries.value.filter((entry) =>
    entry.name.toLowerCase().includes(search.value.toLowerCase())
  )
})

async function loadShinies() {
  const response = await fetch('http://localhost:5119/api/shinies')
  const data = await response.json()

  shinies.value = data
}

onMounted(() => {
  loadShinies()
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

const dexEntries = computed(() => {
  return pokemonDex.map((pokemon) => {
    const ownedShinies = shinies.value.filter((shiny) => {
      const samePokemon =
        shiny.pokemon.toLowerCase() === pokemon.name.toLowerCase()

      if (!samePokemon) {
        return false
      }

      if (pokemon.gender) {
        return shiny.gender === pokemon.gender
      }

      return true
    })

    return {
      ...pokemon,
      obtained: ownedShinies.length > 0,
      shinies: ownedShinies
    }
  })
})

const filteredShinies = computed(() => {
  return shinies.value.filter((shiny) =>
    shiny.pokemon.toLowerCase().includes(search.value.toLowerCase())
  )
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
  v-for="entry in filteredDexEntries"
  :key="`${entry.dexNumber}-${entry.gender ?? 'default'}`"
  :entry="entry"
/>
    </section>
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
</style>

