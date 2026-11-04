<script setup>
import { ref, computed, onMounted } from 'vue'
import { RouterLink } from 'vue-router'
import { gameGroups } from '../data/gameGroups'

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
    errorMessage.value = 'Could not load game statistics.'
  } finally {
    isLoading.value = false
  }
}

onMounted(() => {
  loadShinies()
})

const gameStats = computed(() => {
  return gameGroups
    .map((group) => {
      const matchingShinies = shinies.value.filter(
        (shiny) => group.games.includes(shiny.game)
      )

      const versionBreakdown = group.games
        .map((game) => ({
          game,
          count: matchingShinies.filter(
            (shiny) => shiny.game === game
          ).length
        }))
        .filter((item) => item.count > 0)

      return {
        ...group,
        count: matchingShinies.length,
        versionBreakdown
      }
    })
    .filter((group) => group.count > 0)
    .sort((a, b) => b.count - a.count)
})

const totalShinies = computed(() => {
  return shinies.value.length
})

function getPercentage(count) {
  if (totalShinies.value === 0) {
    return 0
  }

  return Math.round(
    (count / totalShinies.value) * 100
  )
}
</script>

<template>
  <main class="games-page">
    <section class="page-header">
      <h1>Shinies by Game</h1>

      <p>
        See which Pokémon games have contributed the most
        to your shiny collection.
      </p>
    </section>

    <p
      v-if="isLoading"
      class="message"
    >
      Loading game statistics...
    </p>

    <p
      v-else-if="errorMessage"
      class="message error"
    >
      {{ errorMessage }}
    </p>

    <section
      v-else
      class="game-grid"
    >
      <RouterLink
        v-for="group in gameStats"
        :key="group.key"
        class="game-card"
        :to="{
          path: '/',
          query: {
            gameGroup: group.key
          }
        }"
      >
        <div class="cover-wrapper">
          <img
            :src="group.cover"
            :alt="group.name"
            class="game-cover"
          />
        </div>

        <div class="game-info">
          <h2>
            {{ group.name }}
          </h2>

          <strong class="shiny-count">
            {{ group.count }}
            {{ group.count === 1 ? 'shiny' : 'shinies' }}
          </strong>

          <span class="percentage">
            {{ getPercentage(group.count) }}% of collection
          </span>

          <div
            v-if="group.versionBreakdown.length > 1"
            class="version-breakdown"
          >
            <div
              v-for="version in group.versionBreakdown"
              :key="version.game"
              class="version-row"
            >
              <span>
                {{ version.game }}
              </span>

              <strong>
                {{ version.count }}
              </strong>
            </div>
          </div>
        </div>
      </RouterLink>
    </section>
  </main>
</template>

<style scoped>
.games-page {
  max-width: 1200px;
  margin: 0 auto;
  padding: 50px 20px;
}

.page-header {
  text-align: center;
  margin-bottom: 40px;
}

.page-header h1 {
  margin: 0;
  font-size: 2.4rem;
}

.page-header p {
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

.game-grid {
  display: grid;
  grid-template-columns:
    repeat(auto-fit, minmax(280px, 1fr));
  gap: 24px;
}

.game-card {
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 20px;
  overflow: hidden;
  box-shadow: 0 6px 18px rgba(0, 0, 0, 0.07);

  text-decoration: none;
  color: inherit;
  cursor: pointer;

  transition:
    transform 0.2s ease,
    box-shadow 0.2s ease;
}

.game-card:hover {
  transform: translateY(-3px);
  box-shadow: 0 10px 24px rgba(0, 0, 0, 0.12);
}

.cover-wrapper {
  width: 100%;
  aspect-ratio: 3 / 2;
  overflow: hidden;
  background: #f3f4f6;
}

.game-cover {
  width: 100%;
  height: 100%;
  object-fit: cover;
  display: block;
}

.game-info {
  padding: 20px;
  text-align: center;
}

.game-info h2 {
  margin: 0 0 12px;
  font-size: 1.2rem;
}

.shiny-count {
  display: block;
  font-size: 1.6rem;
  color: #111827;
}

.percentage {
  display: block;
  margin-top: 6px;
  color: #6b7280;
  font-size: 0.9rem;
}

.version-breakdown {
  margin-top: 18px;
  padding-top: 14px;
  border-top: 1px solid #e5e7eb;
}

.version-row {
  display: flex;
  justify-content: space-between;
  gap: 12px;
  padding: 5px 0;
  font-size: 0.9rem;
}

.version-row span {
  text-align: left;
  color: #6b7280;
}

@media (max-width: 600px) {
  .games-page {
    padding: 30px 16px;
  }

  .page-header h1 {
    font-size: 2rem;
  }
}
</style>