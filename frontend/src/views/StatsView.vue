<script setup>
import { ref, computed, onMounted } from 'vue'
import { RouterLink } from 'vue-router'
import { pokemonDex } from '../data/pokedex'

const shinies = ref([])
const isLoading = ref(true)
const errorMessage = ref('')

async function loadShinies() {
  try {
    const response = await fetch('http://localhost:5119/api/shinies')

    if (!response.ok) {
      throw new Error('Loading shinies failed')
    }

    shinies.value = await response.json()
  } catch (error) {
    console.error(error)
    errorMessage.value = 'Could not load collection statistics.'
  } finally {
    isLoading.value = false
  }
}

onMounted(() => {
  loadShinies()
})

const totalShinies = computed(() => {
  return shinies.value.length
})

const obtainedDexSlots = computed(() => {
  return pokemonDex.filter((pokemon) => {
    return shinies.value.some((shiny) => {
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
  }).length
})

const completionPercentage = computed(() => {
  if (pokemonDex.length === 0) {
    return 0
  }

  return Math.round(
    (obtainedDexSlots.value / pokemonDex.length) * 100
  )
})

const duplicateCount = computed(() => {
  return Math.max(
    totalShinies.value - obtainedDexSlots.value,
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

const gameBreakdown = computed(() => {
  const counts = {}

  for (const shiny of shinies.value) {
    const gameName = shiny.game || 'Unknown'

    counts[gameName] =
      (counts[gameName] ?? 0) + 1
  }

  return Object.entries(counts)
    .map(([game, count]) => ({
      game,
      count
    }))
    .sort((a, b) => b.count - a.count)
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
            {{ obtainedDexSlots }} / {{ pokemonDex.length }}
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

.stat-link {
  text-decoration: none;
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

.statistics-section {
  margin-top: 40px;
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 18px;
  padding: 28px;
  box-shadow: 0 6px 18px rgba(0, 0, 0, 0.07);
}

.statistics-section h2 {
  margin-top: 0;
}

.game-list {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.game-row {
  display: flex;
  align-items: center;
  padding: 12px 14px;
  border-radius: 10px;
  background: #f9fafb;
}

.game-row strong {
  margin-left: auto;
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

  .stat-link-text {
    font-size: 1.1rem;
  }
}
</style>