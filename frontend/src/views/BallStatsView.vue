<script setup>
import { ref, computed, onMounted } from 'vue'
import { ballSprites } from '../data/ballSprites'

const shinies = ref([])
const isLoading = ref(true)
const errorMessage = ref('')
const visibleCount = ref(10)

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
      'Could not load ball statistics.'
  } finally {
    isLoading.value = false
  }
}

onMounted(() => {
  loadShinies()
})

const ranking = computed(() => {
  const counts = {}

  for (const shiny of shinies.value) {
    if (!shiny.ball) {
      continue
    }

    counts[shiny.ball] =
      (counts[shiny.ball] ?? 0) + 1
  }

  return Object.entries(counts)
    .map(([name, count]) => ({
      name,
      count,
      sprite: ballSprites[name] ?? null
    }))
    .sort((a, b) => b.count - a.count)
})

const podium = computed(() => {
  return ranking.value.slice(0, 3)
})

const visibleRanking = computed(() => {
  return ranking.value.slice(0, visibleCount.value)
})

function loadMore() {
  visibleCount.value += 10
}
</script>

<template>
  <main class="stats-page">
    <section class="page-header">
      <h1>Ball Statistics</h1>

      <p>
        See which Poké Balls you use most often for your
        shiny collection.
      </p>
    </section>

    <p
      v-if="isLoading"
      class="message"
    >
      Loading ball statistics...
    </p>

    <p
      v-else-if="errorMessage"
      class="message error"
    >
      {{ errorMessage }}
    </p>

    <template v-else>
      <section
        v-if="podium.length > 0"
        class="podium-section"
      >
        <div class="podium">
          <article
            v-if="podium[1]"
            class="podium-entry second-place"
          >
            <div class="podium-info">
              <img
                v-if="podium[1].sprite"
                :src="podium[1].sprite"
                :alt="podium[1].name"
                class="ball-sprite"
              />

              <span class="medal">
                #2
              </span>

              <h2>
                {{ podium[1].name }}
              </h2>

              <strong>
                {{ podium[1].count }} shinies
              </strong>
            </div>

            <div class="podium-block">
              2
            </div>
          </article>

          <article
            v-if="podium[0]"
            class="podium-entry first-place"
          >
            <div class="podium-info">
              <img
                v-if="podium[0].sprite"
                :src="podium[0].sprite"
                :alt="podium[0].name"
                class="ball-sprite winner-ball"
              />

              <span class="medal">
                #1
              </span>

              <h2>
                {{ podium[0].name }}
              </h2>

              <strong>
                {{ podium[0].count }} shinies
              </strong>
            </div>

            <div class="podium-block">
              1
            </div>
          </article>

          <article
            v-if="podium[2]"
            class="podium-entry third-place"
          >
            <div class="podium-info">
              <img
                v-if="podium[2].sprite"
                :src="podium[2].sprite"
                :alt="podium[2].name"
                class="ball-sprite"
              />

              <span class="medal">
                #3
              </span>

              <h2>
                {{ podium[2].name }}
              </h2>

              <strong>
                {{ podium[2].count }} shinies
              </strong>
            </div>

            <div class="podium-block">
              3
            </div>
          </article>
        </div>
      </section>

      <section class="ranking-section">
        <h2>Full Ball Ranking</h2>

        <div class="ranking-list">
          <div
            v-for="(ball, index) in visibleRanking"
            :key="ball.name"
            class="ranking-row"
          >
            <span class="rank">
              #{{ index + 1 }}
            </span>

            <div class="ball-name">
              <img
                v-if="ball.sprite"
                :src="ball.sprite"
                :alt="ball.name"
                class="ranking-ball-sprite"
              />

              <strong>
                {{ ball.name }}
              </strong>
            </div>

            <span class="ranking-count">
              {{ ball.count }}
              {{ ball.count === 1 ? 'shiny' : 'shinies' }}
            </span>
          </div>
        </div>

        <button
          v-if="visibleCount < ranking.length"
          class="load-more-button"
          @click="loadMore"
        >
          Load more
        </button>
      </section>
    </template>
  </main>
</template>

<style scoped>
.stats-page {
  max-width: 1000px;
  margin: 0 auto;
  padding: 50px 20px;
}

.page-header {
  text-align: center;
  margin-bottom: 50px;
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

.podium-section {
  margin-bottom: 55px;
}

.podium {
  display: flex;
  justify-content: center;
  align-items: flex-end;
  gap: 18px;
  min-height: 460px;
}

.podium-entry {
  width: 220px;
  display: flex;
  flex-direction: column;
  justify-content: flex-end;
}

.first-place {
  width: 250px;
}

.podium-info {
  margin-bottom: 14px;
  text-align: center;
}

.podium-info h2 {
  margin: 8px 0 5px;
  font-size: 1.4rem;
}

.first-place .podium-info h2 {
  font-size: 1.7rem;
}

.podium-info strong {
  color: #6b7280;
}

.ball-sprite {
  display: block;
  width: 70px;
  height: 70px;
  object-fit: contain;
  margin: 0 auto 10px;

  image-rendering: pixelated;
}

.winner-ball {
  width: 90px;
  height: 90px;
}

.medal {
  display: inline-block;
  padding: 6px 12px;
  border-radius: 999px;
  background: #f3f4f6;
  font-weight: 700;
}

.podium-block {
  display: flex;
  justify-content: center;
  align-items: flex-start;

  padding-top: 25px;

  border-radius: 18px 18px 0 0;

  background: white;

  border: 1px solid #e5e7eb;
  border-bottom: none;

  box-shadow: 0 -4px 18px rgba(0, 0, 0, 0.06);

  font-size: 2rem;
  font-weight: 700;
  color: #6b7280;
}

.first-place .podium-block {
  height: 220px;
}

.second-place .podium-block {
  height: 160px;
}

.third-place .podium-block {
  height: 120px;
}

.ranking-section {
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 20px;
  padding: 28px;

  box-shadow: 0 6px 18px rgba(0, 0, 0, 0.07);
}

.ranking-section h2 {
  margin: 0 0 22px;
  text-align: center;
}

.ranking-list {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.ranking-row {
  display: grid;
  grid-template-columns: 60px 1fr auto;
  align-items: center;
  gap: 14px;

  padding: 12px 16px;

  background: #f9fafb;
  border-radius: 10px;
}

.rank {
  color: #6b7280;
  font-weight: 700;
}

.ball-name {
  display: flex;
  align-items: center;
  gap: 12px;
}

.ranking-ball-sprite {
  width: 38px;
  height: 38px;
  object-fit: contain;

  image-rendering: pixelated;
}

.ranking-count {
  color: #6b7280;
}

.load-more-button {
  display: block;
  margin: 22px auto 0;
  padding: 11px 20px;

  border: none;
  border-radius: 10px;

  background: #111827;
  color: white;

  font-weight: 600;
  cursor: pointer;
}

.load-more-button:hover {
  opacity: 0.9;
}

@media (max-width: 700px) {
  .stats-page {
    padding: 30px 16px;
  }

  .page-header h1 {
    font-size: 2rem;
  }

  .podium {
    gap: 8px;
    min-height: 370px;
  }

  .podium-entry {
    width: 31%;
  }

  .first-place {
    width: 36%;
  }

  .podium-info h2 {
    font-size: 0.95rem;
  }

  .first-place .podium-info h2 {
    font-size: 1.1rem;
  }

  .podium-info strong {
    font-size: 0.75rem;
  }

  .ball-sprite {
    width: 50px;
    height: 50px;
  }

  .winner-ball {
    width: 65px;
    height: 65px;
  }

  .first-place .podium-block {
    height: 170px;
  }

  .second-place .podium-block {
    height: 125px;
  }

  .third-place .podium-block {
    height: 90px;
  }

  .ranking-section {
    padding: 18px 12px;
  }

  .ranking-row {
    grid-template-columns: 40px 1fr auto;
    gap: 8px;

    padding: 10px 8px;

    font-size: 0.85rem;
  }

  .ranking-ball-sprite {
    width: 30px;
    height: 30px;
  }
}
</style>