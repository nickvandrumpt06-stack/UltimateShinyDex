<script setup>
import {
  computed,
  onMounted,
  ref
} from 'vue'

import {
  useRoute,
  useRouter
} from 'vue-router'

import {
  games as gameOptions,
  natures as natureOptions
} from '../data/options'

import { pokemonDex } from '../data/pokedex'

const route = useRoute()
const router = useRouter()

const hunt = ref(null)

const isLoading = ref(true)
const isSaving = ref(false)
const errorMessage = ref('')

const huntName = ref('')
const usesCounter = ref(true)
const counterStep = ref(1)
const notes = ref('')

const gameInstances = ref([])
const targets = ref([])

function createGame() {
  return {
    game: '',
    label: '',
    shinyOdds: 4096,
    encounters: []
  }
}

function createEncounter() {
  return {
    pokemon: '',
    form: '',
    encounterPercentage: null
  }
}

function createTarget() {
  return {
    pokemon: '',
    form: '',
    gender: '',
    nature: '',
    isAlpha: null,
    mark: ''
  }
}

const pokemonNames = computed(() => {
  return [
    ...new Set(
      pokemonDex.map(
        (entry) => entry.name
      )
    )
  ].sort(
    (a, b) =>
      a.localeCompare(b)
  )
})

async function loadHunt() {
  try {
    const response = await fetch(
      `http://localhost:5119/api/hunts/${route.params.id}`
    )

    if (!response.ok) {
      throw new Error(
        'Could not load hunt'
      )
    }

    hunt.value =
      await response.json()

    huntName.value =
      hunt.value.name

    usesCounter.value =
      hunt.value.usesCounter

    counterStep.value =
      hunt.value.counterStep

    notes.value =
      hunt.value.notes ?? ''

    gameInstances.value =
      hunt.value.games.map(
        (game) => ({
          game:
            game.game,

          label:
            game.label ?? '',

          shinyOdds:
            game.shinyOdds,

          encounters:
            (game.encounters ?? [])
              .map(
                (encounter) => ({
                  pokemon:
                    encounter.pokemon,

                  form:
                    encounter.form ?? '',

                  encounterPercentage:
                    encounter
                      .encounterPercentage
                })
              )
        })
      )

    targets.value =
      hunt.value.targets.map(
        (target) => ({
          pokemon:
            target.pokemon,

          form:
            target.form ?? '',

          gender:
            target.gender ?? '',

          nature:
            target.nature ?? '',

          isAlpha:
            target.isAlpha,

          mark:
            target.mark ?? ''
        })
      )
  } catch (error) {
    console.error(error)

    errorMessage.value =
      'Could not load this hunt.'
  } finally {
    isLoading.value = false
  }
}

onMounted(() => {
  loadHunt()
})

function addGame() {
  gameInstances.value.push(
    createGame()
  )
}

function removeGame(index) {
  if (
    gameInstances.value.length <= 1
  ) {
    errorMessage.value =
      'A hunt needs at least one game.'

    return
  }

  gameInstances.value.splice(
    index,
    1
  )
}

function addEncounter(gameIndex) {
  gameInstances.value[
    gameIndex
  ].encounters.push(
    createEncounter()
  )
}

function removeEncounter(
  gameIndex,
  encounterIndex
) {
  gameInstances.value[
    gameIndex
  ].encounters.splice(
    encounterIndex,
    1
  )
}

function copyEncounterPool(
  targetIndex,
  sourceIndex
) {
  if (sourceIndex === '') {
    return
  }

  const source =
    gameInstances.value[
      Number(sourceIndex)
    ]

  gameInstances.value[
    targetIndex
  ].encounters =
    source.encounters.map(
      (encounter) => ({
        pokemon:
          encounter.pokemon,

        form:
          encounter.form,

        encounterPercentage:
          encounter.encounterPercentage
      })
    )
}

function addTarget() {
  targets.value.push(
    createTarget()
  )
}

function removeTarget(index) {
  if (
    targets.value.length <= 1
  ) {
    errorMessage.value =
      'A hunt needs at least one target.'

    return
  }

  targets.value.splice(
    index,
    1
  )
}

function encounterTotal(game) {
  const total =
    game.encounters.reduce(
      (sum, encounter) => {
        if (
          encounter.encounterPercentage ===
            null ||
          encounter.encounterPercentage ===
            ''
        ) {
          return sum
        }

        return (
          sum +
          Number(
            encounter.encounterPercentage
          )
        )
      },
      0
    )

  return (
    Math.round(total * 100) /
    100
  )
}

function validate() {
  errorMessage.value = ''

  if (!huntName.value.trim()) {
    errorMessage.value =
      'Please enter a hunt name.'

    return false
  }

  if (
    usesCounter.value &&
    Number(counterStep.value) < 1
  ) {
    errorMessage.value =
      'Counter step must be at least 1.'

    return false
  }

  if (
    gameInstances.value.length <
    1
  ) {
    errorMessage.value =
      'A hunt needs at least one game.'

    return false
  }

  for (
    const game
    of gameInstances.value
  ) {
    if (!game.game) {
      errorMessage.value =
        'Please select every game.'

      return false
    }

    if (
      game.shinyOdds !== null &&
      game.shinyOdds !== '' &&
      Number(game.shinyOdds) < 1
    ) {
      errorMessage.value =
        'Shiny odds must be at least 1.'

      return false
    }

    const percentages =
      game.encounters.filter(
        (encounter) =>
          encounter
            .encounterPercentage !==
            null &&
          encounter
            .encounterPercentage !==
            ''
      )

    for (
      const encounter
      of game.encounters
    ) {
      if (
        !encounter.pokemon.trim()
      ) {
        errorMessage.value =
          'Every encounter needs a Pokémon.'

        return false
      }
    }

    if (
      percentages.length > 0
    ) {
      if (
        percentages.length !==
        game.encounters.length
      ) {
        errorMessage.value =
          `${game.label || game.game}: if you use percentages, every encounter needs one.`

        return false
      }

      const total =
        encounterTotal(game)

      if (
        Math.abs(
          100 - total
        ) > 0.11
      ) {
        errorMessage.value =
          `${game.label || game.game}: encounter percentages total ${total}%. They should total 100%.`

        return false
      }
    }
  }

  if (
    targets.value.length < 1
  ) {
    errorMessage.value =
      'Please add at least one target.'

    return false
  }

  if (
    targets.value.some(
      (target) =>
        !target.pokemon.trim()
    )
  ) {
    errorMessage.value =
      'Every target needs a Pokémon.'

    return false
  }

  return true
}

async function saveHunt() {
  if (!validate()) {
    return
  }

  isSaving.value = true

  const body = {
    name:
      huntName.value.trim(),

    usesCounter:
      usesCounter.value,

    counterStep:
      usesCounter.value
        ? Number(
            counterStep.value
          )
        : 1,

    notes:
      notes.value.trim()
        ? notes.value.trim()
        : null,

    games:
      gameInstances.value.map(
        (game) => ({
          game:
            game.game,

          label:
            game.label.trim()
              ? game.label.trim()
              : null,

          shinyOdds:
            game.shinyOdds ===
              '' ||
            game.shinyOdds ===
              null
              ? null
              : Number(
                  game.shinyOdds
                ),

          encounters:
            game.encounters.map(
              (encounter) => ({
                pokemon:
                  encounter
                    .pokemon
                    .trim(),

                form:
                  encounter.form.trim()
                    ? encounter
                        .form
                        .trim()
                    : null,

                encounterPercentage:
                  encounter
                    .encounterPercentage ===
                    '' ||
                  encounter
                    .encounterPercentage ===
                    null
                    ? null
                    : Number(
                        encounter
                          .encounterPercentage
                      )
              })
            )
        })
      ),

    targets:
      targets.value.map(
        (target) => ({
          pokemon:
            target.pokemon.trim(),

          form:
            target.form.trim()
              ? target.form.trim()
              : null,

          gender:
            target.gender || null,

          nature:
            target.nature || null,

          isAlpha:
            target.isAlpha,

          mark:
            target.mark.trim()
              ? target.mark.trim()
              : null
        })
      )
  }

  try {
    const response = await fetch(
      `http://localhost:5119/api/hunts/${route.params.id}`,
      {
        method: 'PUT',

        headers: {
          'Content-Type':
            'application/json'
        },

        body:
          JSON.stringify(body)
      }
    )

    if (!response.ok) {
      const result =
        await response.text()

      console.error(result)

      throw new Error(
        'Could not update hunt'
      )
    }

    router.push(
      `/hunts/${route.params.id}`
    )
  } catch (error) {
    console.error(error)

    errorMessage.value =
      'Something went wrong while saving the hunt.'
  } finally {
    isSaving.value = false
  }
}

function cancel() {
  router.push(
    `/hunts/${route.params.id}`
  )
}
</script>

<template>
  <main class="edit-page">
    <p
      v-if="isLoading"
      class="message"
    >
      Loading hunt...
    </p>

    <template v-else-if="hunt">
      <header class="page-header">
        <div>
          <h1>Edit Hunt</h1>

          <p>
            Update the setup without
            resetting your hunt
            progress.
          </p>
        </div>

        <button
          type="button"
          class="secondary-button"
          @click="cancel"
        >
          Cancel
        </button>
      </header>

      <p
        v-if="errorMessage"
        class="error-message"
      >
        {{ errorMessage }}
      </p>

      <!-- BASICS -->
      <section class="edit-section">
        <h2>Basics</h2>

        <div class="field">
          <label>Hunt Name</label>

          <input
            v-model="huntName"
            type="text"
          />
        </div>

        <div class="field">
          <label>
            Encounter Counter
          </label>

          <div class="choice-grid">
            <button
              type="button"
              class="choice-card"
              :class="{
                selected:
                  usesCounter
              }"
              @click="
                usesCounter = true
              "
            >
              <strong>
                Use Counter
              </strong>

              <span>
                Track encounters.
              </span>
            </button>

            <button
              type="button"
              class="choice-card"
              :class="{
                selected:
                  !usesCounter
              }"
              @click="
                usesCounter = false
              "
            >
              <strong>
                No Counter
              </strong>

              <span>
                Only track phases.
              </span>
            </button>
          </div>
        </div>

        <div
          v-if="usesCounter"
          class="field"
        >
          <label>
            Counter Step
          </label>

          <input
            v-model.number="
              counterStep
            "
            type="number"
            min="1"
          />
        </div>

        <div class="field">
          <label>Notes</label>

          <textarea
            v-model="notes"
            rows="4"
            placeholder="Optional notes about this hunt..."
          ></textarea>
        </div>
      </section>

      <!-- GAMES -->
      <section class="edit-section">
        <div class="section-heading">
          <h2>Games</h2>

          <button
            type="button"
            class="outline-button"
            @click="addGame"
          >
            + Add Game
          </button>
        </div>

        <article
          v-for="
            (
              game,
              gameIndex
            ) in gameInstances
          "
          :key="gameIndex"
          class="edit-card"
        >
          <div class="card-heading">
            <h3>
              Game
              {{ gameIndex + 1 }}
            </h3>

            <button
              v-if="
                gameInstances.length >
                1
              "
              type="button"
              class="remove-button"
              @click="
                removeGame(
                  gameIndex
                )
              "
            >
              Remove
            </button>
          </div>

          <div class="game-grid">
            <div class="field">
              <label>Game</label>

              <select
                v-model="game.game"
              >
                <option
                  disabled
                  value=""
                >
                  Choose game
                </option>

                <option
                  v-for="
                    option
                    in gameOptions
                  "
                  :key="option"
                  :value="option"
                >
                  {{ option }}
                </option>
              </select>
            </div>

            <div class="field">
              <label>Label</label>

              <input
                v-model="game.label"
                type="text"
                placeholder="Optional"
              />
            </div>

            <div class="field">
              <label>
                Shiny Odds
              </label>

              <input
                v-model.number="
                  game.shinyOdds
                "
                type="number"
                min="1"
              />
            </div>
          </div>

          <div class="encounter-heading">
            <h4>
              Encounter Pool
            </h4>

            <div class="encounter-actions">
              <select
                v-if="
                  gameInstances.length >
                  1
                "
                value=""
                @change="
                  copyEncounterPool(
                    gameIndex,
                    $event.target.value
                  );
                  $event.target.value = ''
                "
              >
                <option value="">
                  Copy from...
                </option>

                <option
                  v-for="
                    (
                      source,
                      sourceIndex
                    ) in gameInstances
                  "
                  v-show="
                    sourceIndex !==
                    gameIndex
                  "
                  :key="
                    sourceIndex
                  "
                  :value="
                    sourceIndex
                  "
                >
                  {{
                    source.label ||
                    source.game ||
                    `Game ${sourceIndex + 1}`
                  }}
                </option>
              </select>

              <button
                type="button"
                class="outline-button"
                @click="
                  addEncounter(
                    gameIndex
                  )
                "
              >
                + Encounter
              </button>
            </div>
          </div>

          <div
            v-for="
              (
                encounter,
                encounterIndex
              ) in game.encounters
            "
            :key="encounterIndex"
            class="encounter-row"
          >
            <div class="field">
              <label>Pokémon</label>

              <input
                v-model="
                  encounter.pokemon
                "
                type="text"
                list="pokemon-list"
              />
            </div>

            <div class="field">
              <label>Form</label>

              <input
                v-model="
                  encounter.form
                "
                type="text"
                placeholder="Optional"
              />
            </div>

            <div class="field">
              <label>%</label>

              <input
                v-model.number="
                  encounter
                    .encounterPercentage
                "
                type="number"
                min="0"
                max="100"
                step="0.01"
              />
            </div>

            <button
              type="button"
              class="remove-icon"
              @click="
                removeEncounter(
                  gameIndex,
                  encounterIndex
                )
              "
            >
              ×
            </button>
          </div>

          <p
            v-if="
              game.encounters.length
            "
            class="percentage-total"
          >
            Total:
            {{
              encounterTotal(game)
            }}%
          </p>
        </article>
      </section>

      <!-- TARGETS -->
      <section class="edit-section">
        <div class="section-heading">
          <h2>Targets</h2>

          <button
            type="button"
            class="outline-button"
            @click="addTarget"
          >
            + Add Target
          </button>
        </div>

        <article
          v-for="
            (
              target,
              index
            ) in targets
          "
          :key="index"
          class="edit-card"
        >
          <div class="card-heading">
            <h3>
              Target {{ index + 1 }}
            </h3>

            <button
              v-if="
                targets.length > 1
              "
              type="button"
              class="remove-button"
              @click="
                removeTarget(index)
              "
            >
              Remove
            </button>
          </div>

          <div class="target-grid">
            <div class="field">
              <label>Pokémon</label>

              <input
                v-model="
                  target.pokemon
                "
                type="text"
                list="pokemon-list"
              />
            </div>

            <div class="field">
              <label>Form</label>

              <input
                v-model="
                  target.form
                "
                type="text"
                placeholder="Any"
              />
            </div>

            <div class="field">
              <label>Gender</label>

              <select
                v-model="
                  target.gender
                "
              >
                <option value="">
                  Any
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

            <div class="field">
              <label>Nature</label>

              <select
                v-model="
                  target.nature
                "
              >
                <option value="">
                  Any
                </option>

                <option
                  v-for="
                    nature
                    in natureOptions
                  "
                  :key="nature"
                  :value="nature"
                >
                  {{ nature }}
                </option>
              </select>
            </div>

            <div class="field">
              <label>Alpha</label>

              <select
                v-model="
                  target.isAlpha
                "
              >
                <option
                  :value="null"
                >
                  Any
                </option>

                <option
                  :value="true"
                >
                  Must be Alpha
                </option>

                <option
                  :value="false"
                >
                  Must not be Alpha
                </option>
              </select>
            </div>

            <div class="field">
              <label>Mark</label>

              <input
                v-model="
                  target.mark
                "
                type="text"
                placeholder="Any"
              />
            </div>
          </div>
        </article>
      </section>

      <footer class="page-actions">
        <button
          type="button"
          class="secondary-button"
          :disabled="isSaving"
          @click="cancel"
        >
          Cancel
        </button>

        <button
          type="button"
          class="save-button"
          :disabled="isSaving"
          @click="saveHunt"
        >
          {{
            isSaving
              ? 'Saving...'
              : 'Save Changes'
          }}
        </button>
      </footer>

      <datalist id="pokemon-list">
        <option
          v-for="
            pokemon
            in pokemonNames
          "
          :key="pokemon"
          :value="pokemon"
        />
      </datalist>
    </template>
  </main>
</template>

<style scoped>
.edit-page {
  max-width: 1000px;
  margin: 0 auto;
  padding: 50px 20px;
}

.message {
  text-align: center;
  font-weight: 700;
}

.page-header,
.section-heading,
.card-heading,
.encounter-heading,
.page-actions {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 20px;
}

.page-header {
  margin-bottom: 30px;
}

.page-header h1 {
  margin: 0;
  font-size: 2.4rem;
}

.page-header p {
  margin: 6px 0 0;
  color: #6b7280;
}

.edit-section {
  margin-bottom: 28px;
  padding: 25px;

  border: 1px solid #e5e7eb;
  border-radius: 18px;

  background: white;
}

.edit-section > h2,
.section-heading h2 {
  margin-top: 0;
}

.edit-card {
  margin-top: 16px;
  padding: 20px;

  border: 1px solid #e5e7eb;
  border-radius: 14px;

  background: #f9fafb;
}

.card-heading h3,
.encounter-heading h4 {
  margin: 0;
}

.field {
  display: flex;
  flex-direction: column;
  gap: 7px;
}

.field label {
  font-weight: 700;
}

.field input,
.field select,
.field textarea,
.encounter-actions select {
  box-sizing: border-box;
  width: 100%;
  padding: 10px 12px;

  border: 1px solid #d1d5db;
  border-radius: 9px;

  background: white;
  font: inherit;
}

.field textarea {
  resize: vertical;
}

.choice-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 12px;
}

.choice-card {
  padding: 16px;

  border: 2px solid #e5e7eb;
  border-radius: 12px;

  background: white;
  text-align: left;

  cursor: pointer;
}

.choice-card.selected {
  border-color: #111827;
  background: #f9fafb;
}

.choice-card strong,
.choice-card span {
  display: block;
}

.choice-card span {
  margin-top: 4px;

  color: #6b7280;
  font-size: 0.82rem;
}

.game-grid {
  display: grid;
  grid-template-columns:
    1fr 1fr 170px;
  gap: 15px;
}

.encounter-heading {
  margin-top: 22px;
}

.encounter-actions {
  display: flex;
  gap: 8px;
}

.encounter-row {
  display: grid;

  grid-template-columns:
    1fr 1fr 130px 42px;

  align-items: end;
  gap: 10px;

  margin-top: 12px;
}

.target-grid {
  display: grid;
  grid-template-columns:
    repeat(2, 1fr);
  gap: 15px;
}

.outline-button,
.secondary-button,
.remove-button {
  padding: 9px 14px;

  border: 1px solid #d1d5db;
  border-radius: 9px;

  background: white;

  font-weight: 700;
  cursor: pointer;
}

.remove-button,
.remove-icon {
  color: #991b1b;
}

.remove-icon {
  height: 42px;

  border: none;
  border-radius: 9px;

  background: #fee2e2;

  font-size: 1.3rem;
  font-weight: 700;

  cursor: pointer;
}

.percentage-total {
  margin-bottom: 0;

  color: #6b7280;
  font-weight: 700;
}

.error-message {
  padding: 12px 15px;

  border-radius: 10px;

  background: #fee2e2;
  color: #991b1b;

  font-weight: 700;
}

.page-actions {
  margin-top: 30px;
  padding-top: 20px;

  border-top: 1px solid #e5e7eb;
}

.save-button {
  padding: 11px 20px;

  border: none;
  border-radius: 10px;

  background: #111827;
  color: white;

  font-weight: 800;
  cursor: pointer;
}

.save-button:disabled,
.secondary-button:disabled {
  opacity: 0.5;
  cursor: default;
}

@media (max-width: 700px) {
  .edit-page {
    padding: 30px 16px;
  }

  .page-header,
  .section-heading,
  .card-heading,
  .encounter-heading {
    align-items: flex-start;
    flex-direction: column;
  }

  .choice-grid,
  .game-grid,
  .encounter-row,
  .target-grid {
    grid-template-columns: 1fr;
  }

  .encounter-actions {
    width: 100%;
    flex-direction: column;
  }

  .remove-icon {
    width: 100%;
  }
}
</style>