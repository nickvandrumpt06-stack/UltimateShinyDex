<script setup>
import { ref, computed, watch } from 'vue'
import { useRouter } from 'vue-router'
import { natures, games, balls } from '../data/options'
import { pokemonDex } from '../data/pokedex'
import { marks } from '../data/marks'

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
const form = ref('')

const availableForms = computed(() => {
  const matchingEntries = pokemonDex.filter(
    (entry) =>
      entry.name.toLowerCase() === pokemon.value.toLowerCase()
  )

  const forms = matchingEntries
    .map((entry) => entry.form)
    .filter((form) => form)

  return [...new Set(forms)]
})

async function addShiny() {
  const newShiny = {
    pokemon: pokemon.value,
    nickname: nickname.value,
    nature: nature.value,
    game: game.value,
    ball: ball.value,
    method: method.value,
    encounters: encounters.value ? parseInt(encounters.value) : null,
    isAlpha: isAlpha.value,
    mark: mark.value,
    gender: gender.value,
    form: form.value
  }

  await fetch('http://localhost:5119/api/shinies', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify(newShiny)
  })

  router.push('/')
}
</script>

<template>
  <main class="add-page">
    <section class="form-card">
      <div class="form-header">
        <h1>Add a Shiny</h1>
        <p>Add a new shiny Pokémon to your collection.</p>
      </div>

      <form @submit.prevent="addShiny" class="shiny-form">
        <div class="form-group">
          <label for="pokemon">Pokémon</label>
          <input
            type="text"
            id="pokemon"
            v-model="pokemon"
            placeholder="e.g. Charizard"
            required
          />
        </div>

        <div class="form-group">
          <label for="nickname">Nickname</label>
          <input
            type="text"
            id="nickname"
            v-model="nickname"
            placeholder="Optional"
          />
        </div>

        <div class="form-group">
          <label for="nature">Nature</label>
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

        <div class="form-group">
          <label for="game">Game</label>
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

        <div class="form-group">
          <label for="ball">Ball</label>
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

        <div class="form-group">
          <label for="method">Method</label>
          <input
            type="text"
            id="method"
            v-model="method"
            placeholder="Random encounters, Masuda..."
          />
        </div>

        <div class="form-group">
          <label for="encounters">Encounters</label>
          <input
            type="number"
            id="encounters"
            v-model.number="encounters"
            min="0"
            placeholder="Optional"
          />
        </div>

        <div class="form-group">
  <label for="mark">Mark</label>

  <select
    id="mark"
    v-model="mark"
  >
    <option value="">
      No Mark
    </option>

    <option
      v-for="markOption in marks"
      :key="markOption.name"
      :value="markOption.name"
    >
      {{ markOption.name }}
    </option>
  </select>
</div>

        <div class="form-group">
          <label for="gender">Gender</label>
          <select
            id="gender"
            v-model="gender"
          >
            <option value="">
              Not applicable / unknown
            </option>

            <option value="Male">Male</option>
            <option value="Female">Female</option>
            <option value="Genderless">Genderless</option>
          </select>
        </div>

        <div class="form-group">
          <label for="form">Form</label>
          <select
            id="form"
            v-model="form"
          >
            <option value="">
              Normal
            </option>

            <option
              v-for="formOption in availableForms"
              :key="formOption"
              :value="formOption"
            >
              {{ formOption }}
            </option>
          </select>
        </div>

        <div class="form-group checkbox-group">
          <label for="isAlpha">
            <input
              type="checkbox"
              id="isAlpha"
              v-model="isAlpha"
            />
            Alpha Pokémon
          </label>
        </div>

        <div class="form-actions">
          <button type="submit">
            Add Shiny
          </button>
        </div>
      </form>
    </section>
  </main>
</template>

<style scoped>
.add-page {
  max-width: 1000px;
  margin: 0 auto;
  padding: 50px 20px;
}

.form-card {
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 22px;
  padding: 32px;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.08);
}

.form-header {
  text-align: center;
  margin-bottom: 32px;
}

.form-header h1 {
  margin: 0;
  font-size: 2rem;
}

.form-header p {
  margin-top: 8px;
  color: #6b7280;
}

.shiny-form {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 20px;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.form-group label {
  font-weight: 600;
  color: #374151;
}

.form-group input,
.form-group select {
  width: 100%;
  padding: 12px 14px;
  border: 1px solid #d1d5db;
  border-radius: 10px;
  font-size: 0.95rem;
  background: white;
  box-sizing: border-box;
}

.form-group input:focus,
.form-group select:focus {
  outline: none;
  border-color: #111827;
  box-shadow: 0 0 0 3px rgba(17, 24, 39, 0.08);
}

.checkbox-group {
  justify-content: center;
}

.checkbox-group label {
  display: flex;
  align-items: center;
  gap: 10px;
  cursor: pointer;
}

.checkbox-group input {
  width: auto;
}

.form-actions {
  grid-column: 1 / -1;
  display: flex;
  justify-content: center;
  margin-top: 10px;
}

.form-actions button {
  padding: 12px 28px;
  border: none;
  border-radius: 12px;
  background: #111827;
  color: white;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: transform 0.2s ease, opacity 0.2s ease;
}

.form-actions button:hover {
  transform: translateY(-2px);
}

.form-actions button:active {
  transform: translateY(0);
}

@media (max-width: 700px) {
  .shiny-form {
    grid-template-columns: 1fr;
  }

  .form-card {
    padding: 24px;
  }
}
</style>