<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { shinies } from '../data/shinies'
import { natures, games, balls } from '../data/options'

const router = useRouter()

const pokemon = ref('')
const nickname = ref('')
const nature = ref('')
const game = ref('')
const ball = ref('')

function addShiny() {
  shinies.value.push({
    id: shinies.value.length + 1,
    pokemon: pokemon.value,
    nickname: nickname.value,
    nature: nature.value,
    game: game.value,
    ball: ball.value
  })

  router.push('/')
}
</script>

<template>
  <main>
    <h1>Add a Shiny</h1>

    <form @submit.prevent="addShiny">
      <div>
        <label for="pokemon">Pokemon:</label>
        <input
          type="text"
          id="pokemon"
          v-model="pokemon"
          required
        />
      </div>

      <div>
        <label for="nickname">Nickname:</label>
        <input
          type="text"
          id="nickname"
          v-model="nickname"
        />
      </div>

      <div>
        <label for="nature">Nature:</label>

        <select
          id="nature"
          v-model="nature"
          required
        >
          <option disabled value="">
            Select a nature
          </option>

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

        <select
          id="game"
          v-model="game"
          required
        >
          <option disabled value="">
            Select a game
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

      <div>
        <label for="ball">Ball:</label>

        <select
          id="ball"
          v-model="ball"
          required
        >
          <option disabled value="">
            Select a ball
          </option>

          <option
            v-for="ballOption in balls"
            :key="ballOption"
            :value="ballOption"
          >
            {{ ballOption }}
          </option>
        </select>
      </div>

      <button type="submit">
        Add Shiny
      </button>
    </form>
  </main>
</template>