<script setup>
import { ref, computed, watch, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { natures, games, balls } from '../data/options'
import { pokemonDex } from '../data/pokedex'
import { marks } from '../data/marks'

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
const gender = ref('')
const form = ref('')

const selectedMarks = ref([])

const selectedSpriteFile = ref(null)
const currentSpritePath = ref('')

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

watch(pokemon, () => {
  if (
    form.value &&
    !availableForms.value.includes(form.value)
  ) {
    form.value = ''
  }
})

async function loadShiny() {
  const response = await fetch(
    `http://localhost:5119/api/shinies/${route.params.id}`
  )

  const shiny = await response.json()

  pokemon.value = shiny.pokemon
  nickname.value = shiny.nickname ?? ''
  nature.value = shiny.nature
  game.value = shiny.game
  ball.value = shiny.ball
  method.value = shiny.method
  encounters.value = shiny.encounters ?? ''
  isAlpha.value = shiny.isAlpha
  gender.value = shiny.gender ?? ''
  form.value = shiny.form ?? ''

  selectedMarks.value =
    shiny.marks?.map((mark) => mark.markName) ?? []

  currentSpritePath.value =
    shiny.customSpritePath ?? ''
}

function handleSpriteFile(event) {
  const file = event.target.files[0]

  if (!file) {
    selectedSpriteFile.value = null
    return
  }

  selectedSpriteFile.value = file
}

async function uploadSprite() {
  if (!selectedSpriteFile.value) {
    return
  }

  const formData = new FormData()

  formData.append(
    'file',
    selectedSpriteFile.value
  )

  const response = await fetch(
    `http://localhost:5119/api/shinies/${route.params.id}/sprite`,
    {
      method: 'POST',
      body: formData
    }
  )

  if (!response.ok) {
    throw new Error('Sprite upload failed')
  }
}

async function updateShiny() {
  const updatedShiny = {
    pokemon: pokemon.value,
    nickname: nickname.value,
    nature: nature.value,
    game: game.value,
    ball: ball.value,
    method: method.value,
    encounters:
      encounters.value !== ''
        ? parseInt(encounters.value)
        : null,

    isAlpha: isAlpha.value,
    gender: gender.value,
    form: form.value,

    marks: selectedMarks.value.map((markName) => ({
      markName: markName
    }))
  }

  const response = await fetch(
    `http://localhost:5119/api/shinies/${route.params.id}`,
    {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(updatedShiny)
    }
  )

  if (!response.ok) {
    throw new Error('Updating shiny failed')
  }

  await uploadSprite()

  router.push('/')
}

onMounted(() => {
  loadShiny()
})
</script>

<template>
  <main class="edit-page">
    <section class="form-card">
      <div class="form-header">
        <h1>Edit Shiny</h1>
        <p>Update this Pokémon's collection details.</p>
      </div>

      <form
        class="shiny-form"
        @submit.prevent="updateShiny"
      >
        <div class="form-group">
          <label for="pokemon">Pokémon</label>

          <input
            id="pokemon"
            v-model="pokemon"
            type="text"
            required
          />
        </div>

        <div class="form-group">
          <label for="nickname">Nickname</label>

          <input
            id="nickname"
            v-model="nickname"
            type="text"
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
            id="method"
            v-model="method"
            type="text"
          />
        </div>

        <div class="form-group">
          <label for="encounters">
            Encounters
          </label>

          <input
            id="encounters"
            v-model.number="encounters"
            type="number"
            min="0"
          />
        </div>

        <div class="form-group">
          <label for="gender">Gender</label>

          <select
            id="gender"
            v-model="gender"
          >
            <option value="">
              Unknown / Not applicable
            </option>

            <option value="Male">
              Male
            </option>

            <option value="Female">
              Female
            </option>

            <option value="Genderless">
              Genderless
            </option>
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
              id="isAlpha"
              v-model="isAlpha"
              type="checkbox"
            />

            Alpha Pokémon
          </label>
        </div>

        <div class="form-group marks-group">
          <label>Marks</label>

          <div class="marks-list">
            <label
              v-for="markOption in marks"
              :key="markOption.name"
              class="mark-option"
            >
              <input
                v-model="selectedMarks"
                type="checkbox"
                :value="markOption.name"
              />

              <img
                :src="markOption.image"
                :alt="markOption.name"
                class="mark-option-icon"
              />

              <span>
                {{ markOption.name }}
              </span>
            </label>
          </div>
        </div>

        <div class="form-group sprite-upload">
          <label for="sprite">
            Custom Sprite / Image
          </label>

          <input
            id="sprite"
            type="file"
            accept="image/png,image/jpeg,image/webp,image/gif"
            @change="handleSpriteFile"
          />

          <p v-if="currentSpritePath" class="sprite-note">
            This shiny currently has a custom image.
            Selecting another file will replace it.
          </p>

          <p v-else class="sprite-note">
            Leave empty to use the default PokéAPI shiny sprite.
          </p>
        </div>

        <div class="form-actions">
          <button type="submit">
            Save Changes
          </button>
        </div>
      </form>
    </section>
  </main>
</template>

<style scoped>
.edit-page {
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

.form-group > label {
  font-weight: 600;
  color: #374151;
}

.form-group input[type="text"],
.form-group input[type="number"],
.form-group input[type="file"],
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

.marks-group {
  grid-column: 1 / -1;
}

.marks-list {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 10px;
  max-height: 260px;
  overflow-y: auto;
  padding: 12px;
  border: 1px solid #d1d5db;
  border-radius: 12px;
  background: #f9fafb;
}

.mark-option {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 8px;
  border-radius: 8px;
  cursor: pointer;
  background: white;
}

.mark-option:hover {
  background: #f3f4f6;
}

.mark-option-icon {
  width: 28px;
  height: 28px;
  object-fit: contain;
}

.sprite-upload {
  grid-column: 1 / -1;
}

.sprite-note {
  margin: 0;
  color: #6b7280;
  font-size: 0.85rem;
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
}

.form-actions button:hover {
  opacity: 0.9;
}

@media (max-width: 700px) {
  .shiny-form {
    grid-template-columns: 1fr;
  }

  .marks-list {
    grid-template-columns: 1fr;
  }

  .form-card {
    padding: 24px;
  }
}
</style>