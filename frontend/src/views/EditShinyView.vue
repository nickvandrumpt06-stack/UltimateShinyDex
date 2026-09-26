<script setup>
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { natures, games, balls } from '../data/options'

const route = useRoute()
const router = useRouter()

const pokemon = ref('')
const nickname = ref('')
const nature = ref('')
const game = ref('')
const ball = ref('')
const method = ref('')
const encounters = ref('')
const isAlpha = ref(false)
const mark = ref('')
const gender = ref('')

async function loadShiny() {
  const response = await fetch(
    `http://localhost:5119/api/shinies/${route.params.id}`
  )

  const shiny = await response.json()

  pokemon.value = shiny.pokemon
  nickname.value = shiny.nickname
  nature.value = shiny.nature
  game.value = shiny.game
  ball.value = shiny.ball
  method.value = shiny.method
  encounters.value = shiny.encounters?.toString()
  isAlpha.value = shiny.isAlpha
  mark.value = shiny.mark
  gender.value = shiny.gender ?? ''
}

async function updateShiny() {
  const updatedShiny = {
    pokemon: pokemon.value,
    nickname: nickname.value,
    nature: nature.value,
    game: game.value,
    ball: ball.value,
    method: method.value,
    encounters: encounters.value ? parseInt(encounters.value) : null,
    isAlpha: isAlpha.value,
    mark: mark.value
  }

  await fetch(
    `http://localhost:5119/api/shinies/${route.params.id}`,
    {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(updatedShiny)
    }
  )

  router.push('/')
}

onMounted(() => {
  loadShiny()
})
</script>

<template>
  <main>
    <h1>Edit Shiny</h1>

    <form @submit.prevent="updateShiny">
      <div>
        <label for="pokemon">Pokemon:</label>
        <input
          id="pokemon"
          v-model="pokemon"
          type="text"
          required
        />
      </div>

      <div>
        <label for="nickname">Nickname:</label>
        <input
          id="nickname"
          v-model="nickname"
          type="text"
        />
      </div>

      <div>
        <label for="nature">Nature:</label>

        <select id="nature" v-model="nature" required>
          <option
            v-for="natureOption in natures"
            :key="natureOption"
            :value="natureOption"
          >
            {{ natureOption }}
          </option>
        </select>
      </div>

      <div>
        <label for="game">Game:</label>

        <select id="game" v-model="game" required>
          <option
            v-for="gameOption in games"
            :key="gameOption"
            :value="gameOption"
          >
            {{ gameOption }}
          </option>
        </select>
      </div>

      <div>
        <label for="ball">Ball:</label>

        <select id="ball" v-model="ball" required>
          <option
            v-for="ballOption in balls"
            :key="ballOption"
            :value="ballOption"
          >
            {{ ballOption }}
          </option>
        </select>
      </div>

      <div>
        <label for="method">Method:</label>
        <input
          id="method"
          v-model="method"
          type="text"
        />

      </div>
      <div>
        <label for="encounters">Encounters:</label>
        <input
          id="encounters"
          v-model="encounters"
          type="number"
          min="0"
        />

      </div>
      <div>
        <label for="isAlpha">Is Alpha:</label>
        <input
          id="isAlpha"
          v-model="isAlpha"
          type="checkbox"
        />
      </div>

      <div>
        <label for="mark">Mark:</label>
        <input
          id="mark"
          v-model="mark"
          type="text"
        />
      </div>

      <div>
  <label for="gender">Gender:</label>

  <select id="gender" v-model="gender">
    <option value="">Unknown / Not applicable</option>
    <option value="Male">Male</option>
    <option value="Female">Female</option>
    <option value="Genderless">Genderless</option>
  </select>
</div>

      <button type="submit">
        Save Changes
      </button>
    </form>
  </main>
</template>