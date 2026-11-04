<script setup>
import { ref, computed, onMounted } from 'vue'
import { pokemonDex } from '../data/pokedex'
import {
  formSprites,
  shinyFemale
} from '../data/formSprites'

const shinies = ref([])
const isLoading = ref(true)
const errorMessage = ref('')
const visibleCount = ref(10)

async function loadShinies() {
  try {
    const response = await fetch('http://localhost:5119/api/shinies')

    if (!response.ok) {
      throw new Error('Loading shinies failed')
    }

    shinies.value = await response.json()
  } catch (error) {
    console.error(error)
    errorMessage.value = 'Could not load Pokémon ranking.'
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
    counts[shiny.pokemon] =
      (counts[shiny.pokemon] ?? 0) + 1
  }

  return Object.entries(counts)
    .map(([pokemon, count]) => ({
      pokemon,
      count
    }))
    .sort((a, b) => b.count - a.count)
})

const topFive = computed(() => {
  return ranking.value.slice(0, 5)
})

const visibleRanking = computed(() => {
  return ranking.value.slice(0, visibleCount.value)
})

function loadMore() {
  visibleCount.value += 10
}

function getSpriteUrl(pokemonName) {
  const shiny = shinies.value.find(
    (item) =>
      item.pokemon.toLowerCase() ===
      pokemonName.toLowerCase()
  )

  if (!shiny) {
    return null
  }

  const dexEntry = pokemonDex.find((entry) => {
    const samePokemon =
      entry.name.toLowerCase() ===
      shiny.pokemon.toLowerCase()

    if (!samePokemon) {
      return false
    }

    const sameForm =
      (entry.form ?? '') ===
      (shiny.form ?? '')

    const sameGender =
      !entry.gender ||
      entry.gender === shiny.gender

    return sameForm && sameGender
  })

  if (!dexEntry) {
    const fallbackEntry = pokemonDex.find(
      (entry) =>
        entry.name.toLowerCase() ===
        pokemonName.toLowerCase()
    )

    if (!fallbackEntry) {
      return null
    }

    return `https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/shiny/${fallbackEntry.dexNumber}.png`
  }

  const genderSpriteKey =
    `${dexEntry.name}|${dexEntry.form ?? ''}|${dexEntry.gender ?? ''}`

  const genderFormSprite =
    formSprites[genderSpriteKey]

  if (genderFormSprite) {
    return genderFormSprite
  }

  const spriteKey =
    `${dexEntry.name}|${dexEntry.form ?? ''}`

  const formSprite =
    formSprites[spriteKey]

  if (formSprite) {
    return formSprite
  }

  if (dexEntry.gender === 'Female') {
    return shinyFemale(dexEntry.dexNumber)
  }

  return `https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/shiny/${dexEntry.dexNumber}.png`
}
</script>

<template>
  <main class="ranking-page">
    <section class="page-header">
      <h1>Most Owned Pokémon</h1>

      <p>
        The Pokémon you somehow keep finding over and over again.
      </p>
    </section>

    <p
      v-if="isLoading"
      class="message"
    >
      Loading ranking...
    </p>

    <p
      v-else-if="errorMessage"
      class="message error"
    >
      {{ errorMessage }}
    </p>

    <template v-else>
      <section
        v-if="topFive.length >= 3"
        class="podium"
      >
        <article class="podium-place second">
          <div class="pokemon-info">
            <img
              :src="getSpriteUrl(topFive[1].pokemon)"
              :alt="`Shiny ${topFive[1].pokemon}`"
              class="podium-sprite"
            />

            <h2>#2</h2>

            <p class="pokemon-name">
              {{ topFive[1].pokemon }}
            </p>

            <strong>
              {{ topFive[1].count }} shinies
            </strong>
          </div>

          <div class="podium-block">
            2
          </div>
        </article>

        <article class="podium-place first">
          <div class="pokemon-info">
            <img
              :src="getSpriteUrl(topFive[0].pokemon)"
              :alt="`Shiny ${topFive[0].pokemon}`"
              class="podium-sprite first-sprite"
            />

            <h2>#1</h2>

            <p class="pokemon-name">
              {{ topFive[0].pokemon }}
            </p>

            <strong>
              {{ topFive[0].count }} shinies
            </strong>
          </div>

          <div class="podium-block">
            1
          </div>
        </article>

        <article class="podium-place third">
          <div class="pokemon-info">
            <img
              :src="getSpriteUrl(topFive[2].pokemon)"
              :alt="`Shiny ${topFive[2].pokemon}`"
              class="podium-sprite"
            />

            <h2>#3</h2>

            <p class="pokemon-name">
              {{ topFive[2].pokemon }}
            </p>

            <strong>
              {{ topFive[2].count }} shinies
            </strong>
          </div>

          <div class="podium-block">
            3
          </div>
        </article>
      </section>

      <section
        v-if="topFive.length >= 5"
        class="lower-ranking"
      >
        <article class="lower-card">
          <img
            :src="getSpriteUrl(topFive[3].pokemon)"
            :alt="`Shiny ${topFive[3].pokemon}`"
            class="lower-sprite"
          />

          <span class="rank-number">
            #4
          </span>

          <h2>
            {{ topFive[3].pokemon }}
          </h2>

          <strong>
            {{ topFive[3].count }} shinies
          </strong>
        </article>

        <article class="lower-card">
          <img
            :src="getSpriteUrl(topFive[4].pokemon)"
            :alt="`Shiny ${topFive[4].pokemon}`"
            class="lower-sprite"
          />

          <span class="rank-number">
            #5
          </span>

          <h2>
            {{ topFive[4].pokemon }}
          </h2>

          <strong>
            {{ topFive[4].count }} shinies
          </strong>
        </article>
      </section>

      <section class="full-ranking">
        <h2>Full Ranking</h2>

        <div
          v-for="(item, index) in visibleRanking"
          :key="item.pokemon"
          class="ranking-row"
        >
          <span class="ranking-number">
            #{{ index + 1 }}
          </span>

          <span class="ranking-name">
            {{ item.pokemon }}
          </span>

          <strong>
            {{ item.count }}
          </strong>
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
.ranking-page {
  max-width: 1100px;
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

.podium {
  display: flex;
  justify-content: center;
  align-items: flex-end;
  gap: 18px;
  margin-top: 40px;
}

.podium-place {
  width: 230px;
  text-align: center;
}

.pokemon-info {
  margin-bottom: 14px;
}

.podium-sprite {
  width: 120px;
  height: 120px;
  object-fit: contain;
  image-rendering: pixelated;
}

.first-sprite {
  width: 145px;
  height: 145px;
}

.pokemon-info h2 {
  margin: 0;
  font-size: 1.7rem;
}

.pokemon-name {
  margin: 6px 0;
  font-size: 1.2rem;
  font-weight: 700;
}

.podium-block {
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 18px 18px 0 0;
  font-size: 2rem;
  font-weight: 700;
  background: #f3f4f6;
  border: 1px solid #e5e7eb;
}

.first .podium-block {
  height: 220px;
}

.second .podium-block {
  height: 170px;
}

.third .podium-block {
  height: 140px;
}

.lower-ranking {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 20px;
  max-width: 600px;
  margin: 30px auto 0;
}

.lower-card {
  padding: 22px;
  text-align: center;
  border: 1px solid #e5e7eb;
  border-radius: 16px;
  background: white;
  box-shadow: 0 6px 18px rgba(0, 0, 0, 0.07);
}

.lower-card h2 {
  margin: 8px 0;
}

.lower-sprite {
  display: block;
  width: 100px;
  height: 100px;
  object-fit: contain;
  image-rendering: pixelated;
  margin: 0 auto 8px;
}

.rank-number {
  color: #6b7280;
  font-weight: 700;
}

.full-ranking {
  margin-top: 50px;
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 18px;
  padding: 28px;
  box-shadow: 0 6px 18px rgba(0, 0, 0, 0.07);
}

.full-ranking h2 {
  margin-top: 0;
}

.ranking-row {
  display: flex;
  align-items: center;
  padding: 12px 14px;
  border-radius: 10px;
  background: #f9fafb;
  margin-bottom: 8px;
}

.ranking-number {
  width: 55px;
  font-weight: 700;
  color: #6b7280;
}

.ranking-name {
  flex: 1;
  font-weight: 600;
}

.load-more-button {
  display: block;
  margin: 20px auto 0;
  padding: 10px 18px;
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

@media (max-width: 750px) {
  .podium {
    gap: 8px;
  }

  .podium-place {
    width: 30%;
  }

  .podium-sprite {
    width: 90px;
    height: 90px;
  }

  .first-sprite {
    width: 110px;
    height: 110px;
  }

  .first .podium-block {
    height: 170px;
  }

  .second .podium-block {
    height: 135px;
  }

  .third .podium-block {
    height: 110px;
  }

  .pokemon-name {
    font-size: 1rem;
  }

  .lower-ranking {
    grid-template-columns: 1fr;
  }
}
</style>