<script setup>
import {
  ref,
  computed,
  watch,
  onMounted
} from 'vue'

import {
  useRoute,
  useRouter
} from 'vue-router'

import {
  natures,
  balls
} from '../data/options'

import { pokemonDex } from '../data/pokedex'
import { marksAndRibbons } from '../data/marks'

const route = useRoute()
const router = useRouter()

const hunt = ref(null)
const isLoading = ref(true)
const isSubmitting = ref(false)

const errorMessage = ref('')

const currentStep = ref(1)

// STEP 1
const wasTarget = ref(null)

// STEP 2
const huntGameId = ref(null)
const pokemon = ref('')
const selectedEncounterId = ref(null)
const useOtherPokemon = ref(false)

// STEP 3
const nickname = ref('')
const nature = ref('')
const ball = ref('')
const method = ref('')
const gender = ref('')
const form = ref('')
const isAlpha = ref(false)
const selectedMarks = ref([])

const defaultFormOverrides = {
  Pumpkaboo: 'Average Size',
  Gourgeist: 'Average Size'
}

const selectedGame = computed(() => {
  if (!hunt.value) {
    return null
  }

  return hunt.value.games.find(
    (game) =>
      game.id === huntGameId.value
  ) ?? null
})

const availableForms = computed(() => {
  const matchingEntries =
    pokemonDex.filter(
      (entry) =>
        entry.name.toLowerCase() ===
        pokemon.value.toLowerCase()
    )

  const forms =
    matchingEntries
      .map((entry) => entry.form)
      .filter((entryForm) => entryForm)

  return [
    ...new Set(forms)
  ]
})

const hasDefaultForm = computed(() => {
  return pokemonDex.some(
    (entry) =>
      entry.name.toLowerCase() ===
        pokemon.value.toLowerCase() &&
      !entry.form
  )
})

const availableMarks = computed(() => {
  return marksAndRibbons.filter(
    (item) =>
      item.type === 'Mark'
  )
})

const availableRibbons = computed(() => {
  return marksAndRibbons.filter(
    (item) =>
      item.type === 'Ribbon'
  )
})

const currentPhaseLabel = computed(() => {
  if (!hunt.value) {
    return ''
  }

  return `Phase ${hunt.value.currentPhaseNumber}`
})

watch(pokemon, (newPokemon) => {
  const matchingEntries =
    pokemonDex.filter(
      (entry) =>
        entry.name.toLowerCase() ===
        newPokemon.toLowerCase()
    )

  if (
    matchingEntries.length === 0
  ) {
    form.value = ''
    return
  }

  const override =
    defaultFormOverrides[newPokemon]

  if (override) {
    form.value = override
    return
  }

  const hasDefaultEntry =
    matchingEntries.some(
      (entry) =>
        !entry.form
    )

  if (hasDefaultEntry) {
    form.value = ''
    return
  }

  const firstForm =
    matchingEntries.find(
      (entry) =>
        entry.form
    )

  form.value =
    firstForm?.form ?? ''
})

async function loadHunt() {
  try {
    const response =
      await fetch(
        `http://localhost:5119/api/hunts/${route.params.id}`
      )

    if (!response.ok) {
      throw new Error(
        'Could not load hunt'
      )
    }

    hunt.value =
      await response.json()

    if (
      hunt.value.status ===
      'Completed'
    ) {
      errorMessage.value =
        'This hunt has already been completed.'

      return
    }

    if (
      hunt.value.games.length === 1
    ) {
      huntGameId.value =
        hunt.value.games[0].id
    }
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

function chooseResult(value) {
  wasTarget.value = value
  errorMessage.value = ''
}

function chooseGame(game) {
  huntGameId.value = game.id

  selectedEncounterId.value = null
  pokemon.value = ''
  useOtherPokemon.value = false

  errorMessage.value = ''
}

function chooseEncounter(encounter) {
  selectedEncounterId.value =
    encounter.id

  pokemon.value =
    encounter.pokemon

  form.value =
    encounter.form ?? ''

  useOtherPokemon.value = false

  errorMessage.value = ''
}

function chooseTarget(target) {
  pokemon.value =
    target.pokemon

  form.value =
    target.form ?? ''

  if (target.gender) {
    gender.value =
      target.gender
  }

  if (target.nature) {
    nature.value =
      target.nature
  }

  if (
    target.isAlpha !== null &&
    target.isAlpha !== undefined
  ) {
    isAlpha.value =
      target.isAlpha
  }

  if (target.mark) {
    if (
      !selectedMarks.value.includes(
        target.mark
      )
    ) {
      selectedMarks.value.push(
        target.mark
      )
    }
  }

  selectedEncounterId.value = null
  useOtherPokemon.value = false

  errorMessage.value = ''
}

function chooseOtherPokemon() {
  pokemon.value = ''
  form.value = ''

  selectedEncounterId.value = null
  useOtherPokemon.value = true

  errorMessage.value = ''
}

function validateStepOne() {
  if (
    wasTarget.value === null
  ) {
    errorMessage.value =
      'Choose whether this shiny was a target or a phase.'

    return false
  }

  return true
}

function validateStepTwo() {
  if (!huntGameId.value) {
    errorMessage.value =
      'Choose which game the shiny was found in.'

    return false
  }

  if (
    !pokemon.value.trim()
  ) {
    errorMessage.value =
      'Choose or enter the Pokémon you found.'

    return false
  }

  return true
}

function validateStepThree() {
  if (!nature.value) {
    errorMessage.value =
      'Choose a nature.'

    return false
  }

  if (!ball.value) {
    errorMessage.value =
      'Choose a Poké Ball.'

    return false
  }

  return true
}

function nextStep() {
  errorMessage.value = ''

  if (
    currentStep.value === 1 &&
    !validateStepOne()
  ) {
    return
  }

  if (
    currentStep.value === 2 &&
    !validateStepTwo()
  ) {
    return
  }

  if (
    currentStep.value === 3 &&
    !validateStepThree()
  ) {
    return
  }

  if (
    currentStep.value < 4
  ) {
    currentStep.value++
  }
}

function previousStep() {
  errorMessage.value = ''

  if (
    currentStep.value > 1
  ) {
    currentStep.value--
  }
}

function cancel() {
  router.push(
    `/hunts/${route.params.id}`
  )
}

function gameDisplayName(game) {
  return game.label || game.game
}

async function saveShinyFound(
  continueHunt
) {
  errorMessage.value = ''

  if (
    !validateStepOne() ||
    !validateStepTwo() ||
    !validateStepThree()
  ) {
    return
  }

  isSubmitting.value = true

  const body = {
    wasTarget:
      wasTarget.value,

    continueHunt,

    huntGameId:
      huntGameId.value,

    shiny: {
      pokemon:
        pokemon.value.trim(),

      nickname:
        nickname.value.trim()
          ? nickname.value.trim()
          : null,

      nature:
        nature.value,

      ball:
        ball.value,

      method:
        method.value.trim(),

      isAlpha:
        isAlpha.value,

      gender:
        gender.value ||
        null,

      form:
        form.value ||
        null,

      marks:
        selectedMarks.value.map(
          (markName) => ({
            markName
          })
        )
    }
  }

  try {
    const response =
      await fetch(
        `http://localhost:5119/api/hunts/${hunt.value.id}/shiny-found`,
        {
          method: 'POST',

          headers: {
            'Content-Type':
              'application/json'
          },

          body:
            JSON.stringify(body)
        }
      )

    if (!response.ok) {
      const message =
        await response.text()

      console.error(message)

      throw new Error(
        'Could not save shiny'
      )
    }

    await response.json()

    router.push(
      `/hunts/${hunt.value.id}`
    )
  } catch (error) {
    console.error(error)

    errorMessage.value =
      'Something went wrong while saving the shiny.'
  } finally {
    isSubmitting.value = false
  }
}
</script>

<template>
  <main class="found-page">
    <p
      v-if="isLoading"
      class="loading"
    >
      Loading hunt...
    </p>

    <template v-else-if="hunt">
      <header class="page-header">
        <div>
          <button
            type="button"
            class="back-button"
            @click="cancel"
          >
            ← Back to Hunt
          </button>

          <h1>
            ✨ Shiny Found
          </h1>

          <p>
            {{ hunt.name }}
            •
            {{ currentPhaseLabel }}
          </p>
        </div>
      </header>

      <section class="steps">
        <div
          v-for="step in 4"
          :key="step"
          class="step-wrapper"
        >
          <div
            class="step"
            :class="{
              active:
                currentStep === step,

              complete:
                currentStep > step
            }"
          >
            <span>
              {{ step }}
            </span>

            {{
              [
                'Result',
                'Pokémon',
                'Details',
                'Finish'
              ][step - 1]
            }}
          </div>

          <div
            v-if="step < 4"
            class="step-line"
          ></div>
        </div>
      </section>

      <section class="card">
        <p
          v-if="errorMessage"
          class="error-message"
        >
          {{ errorMessage }}
        </p>

        <!-- STEP 1 -->
        <div
          v-if="currentStep === 1"
          class="step-content"
        >
          <div class="section-heading">
            <span>Step 1</span>

            <h2>
              What did you find?
            </h2>

            <p>
              Was this one of your hunt
              targets, or another shiny
              found along the way?
            </p>
          </div>

          <div class="result-options">
            <button
              type="button"
              class="result-card target-result"
              :class="{
                selected:
                  wasTarget === true
              }"
              @click="
                chooseResult(true)
              "
            >
              <strong>
                🎯 Target
              </strong>

              <span>
                This shiny satisfies one
                of the targets for this
                hunt.
              </span>
            </button>

            <button
              type="button"
              class="result-card"
              :class="{
                selected:
                  wasTarget === false
              }"
              @click="
                chooseResult(false)
              "
            >
              <strong>
                ✨ Phase
              </strong>

              <span>
                A shiny appeared, but it
                wasn't the target.
              </span>
            </button>
          </div>

          <div
            v-if="
              wasTarget === true &&
              hunt.targets?.length
            "
            class="target-preview"
          >
            <span>
              Hunt targets
            </span>

            <div class="preview-tags">
              <strong
                v-for="
                  target
                  in hunt.targets
                "
                :key="target.id"
              >
                {{ target.pokemon }}
              </strong>
            </div>
          </div>
        </div>

        <!-- STEP 2 -->
        <div
          v-if="currentStep === 2"
          class="step-content"
        >
          <div class="section-heading">
            <span>Step 2</span>

            <h2>
              Which Pokémon did you find?
            </h2>

            <p>
              Choose the game instance
              first, then the shiny.
            </p>
          </div>

          <div
            v-if="
              hunt.games.length > 1
            "
            class="section-block"
          >
            <h3>
              Game Instance
            </h3>

            <div class="option-grid">
              <button
                v-for="
                  game in hunt.games
                "
                :key="game.id"
                type="button"
                class="select-card"
                :class="{
                  selected:
                    huntGameId ===
                    game.id
                }"
                @click="
                  chooseGame(game)
                "
              >
                <strong>
                  {{
                    gameDisplayName(
                      game
                    )
                  }}
                </strong>

                <span>
                  {{ game.game }}

                  <template
                    v-if="
                      game.shinyOdds
                    "
                  >
                    • 1 /
                    {{ game.shinyOdds }}
                  </template>
                </span>
              </button>
            </div>
          </div>

          <div
            v-else-if="selectedGame"
            class="single-game"
          >
            Found in

            <strong>
              {{
                gameDisplayName(
                  selectedGame
                )
              }}
            </strong>
          </div>

          <template v-if="selectedGame">
            <div
              v-if="
                wasTarget === true &&
                hunt.targets?.length
              "
              class="section-block"
            >
              <h3>
                Targets
              </h3>

              <p class="help-text">
                Selecting a target will
                pre-fill any target
                requirements you set
                when creating the hunt.
              </p>

              <div class="pokemon-grid">
                <button
                  v-for="
                    target
                    in hunt.targets
                  "
                  :key="target.id"
                  type="button"
                  class="pokemon-card"
                  :class="{
                    selected:
                      pokemon ===
                        target.pokemon &&
                      form ===
                        (target.form || '')
                  }"
                  @click="
                    chooseTarget(target)
                  "
                >
                  <strong>
                    {{ target.pokemon }}
                  </strong>

                  <span
                    v-if="target.form"
                  >
                    {{ target.form }}
                  </span>

                  <span
                    v-if="target.gender"
                  >
                    {{ target.gender }}
                  </span>
                </button>
              </div>
            </div>

            <div
              v-if="
                selectedGame.encounters
                  ?.length
              "
              class="section-block"
            >
              <h3>
                Encounter Pool
              </h3>

              <div class="pokemon-grid">
                <button
                  v-for="
                    encounter
                    in selectedGame.encounters
                  "
                  :key="encounter.id"
                  type="button"
                  class="pokemon-card"
                  :class="{
                    selected:
                      selectedEncounterId ===
                      encounter.id
                  }"
                  @click="
                    chooseEncounter(
                      encounter
                    )
                  "
                >
                  <strong>
                    {{
                      encounter.pokemon
                    }}
                  </strong>

                  <span
                    v-if="
                      encounter.form
                    "
                  >
                    {{ encounter.form }}
                  </span>

                  <span
                    v-if="
                      encounter.encounterPercentage !==
                      null
                    "
                  >
                    {{
                      encounter.encounterPercentage
                    }}%
                  </span>
                </button>

                <button
                  type="button"
                  class="pokemon-card"
                  :class="{
                    selected:
                      useOtherPokemon
                  }"
                  @click="
                    chooseOtherPokemon
                  "
                >
                  <strong>
                    Other Pokémon
                  </strong>

                  <span>
                    Enter manually
                  </span>
                </button>
              </div>
            </div>

            <div
              v-else
              class="section-block"
            >
              <h3>
                Pokémon
              </h3>

              <p class="help-text">
                This game doesn't have
                an encounter pool, so
                enter the Pokémon
                manually.
              </p>

              <button
                type="button"
                class="outline-button"
                @click="
                  chooseOtherPokemon
                "
              >
                Enter Pokémon
              </button>
            </div>

            <div
              v-if="
                useOtherPokemon ||
                (
                  !selectedGame.encounters
                    ?.length &&
                  !pokemon
                )
              "
              class="manual-pokemon"
            >
              <label>
                Pokémon
              </label>

              <input
                v-model="pokemon"
                type="text"
                list="all-pokemon"
                placeholder="e.g. Meowth"
              />
            </div>

            <div
              v-if="pokemon"
              class="selected-pokemon"
            >
              Selected:

              <strong>
                {{ pokemon }}

                <template v-if="form">
                  • {{ form }}
                </template>
              </strong>
            </div>
          </template>
        </div>

        <!-- STEP 3 -->
        <div
          v-if="currentStep === 3"
          class="step-content"
        >
          <div class="section-heading">
            <span>Step 3</span>

            <h2>
              Shiny Details
            </h2>

            <p>
              These details are saved to
              your normal shiny
              collection as well.
            </p>
          </div>

          <div class="found-summary">
            <strong>
              ✨ {{ pokemon }}
            </strong>

            <span>
              {{
                gameDisplayName(
                  selectedGame
                )
              }}
            </span>

            <span
              v-if="hunt.usesCounter"
            >
              Found after
              {{
                hunt.currentEncounters
                  .toLocaleString()
              }}
              encounters this phase
            </span>
          </div>

          <div class="form-grid">
            <div class="form-group">
              <label>
                Nickname
              </label>

              <input
                v-model="nickname"
                type="text"
                placeholder="Optional"
              />
            </div>

            <div class="form-group">
              <label>
                Nature
              </label>

              <select
                v-model="nature"
              >
                <option
                  disabled
                  value=""
                >
                  Select a nature
                </option>

                <option
                  v-for="
                    natureOption
                    in natures
                  "
                  :key="
                    natureOption
                  "
                  :value="
                    natureOption
                  "
                >
                  {{ natureOption }}
                </option>
              </select>
            </div>

            <div class="form-group">
              <label>
                Ball
              </label>

              <select
                v-model="ball"
              >
                <option
                  disabled
                  value=""
                >
                  Select a ball
                </option>

                <option
                  v-for="
                    ballOption
                    in balls
                  "
                  :key="
                    ballOption
                  "
                  :value="
                    ballOption
                  "
                >
                  {{ ballOption }}
                </option>
              </select>
            </div>

            <div class="form-group">
              <label>
                Method
              </label>

              <input
                v-model="method"
                type="text"
                placeholder="Random encounters, Masuda..."
              />
            </div>

            <div class="form-group">
              <label>
                Gender
              </label>

              <select
                v-model="gender"
              >
                <option value="">
                  Not applicable / unknown
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
              <label>
                Form
              </label>

              <select
                v-if="
                  availableForms.length ||
                  hasDefaultForm
                "
                v-model="form"
              >
                <option
                  v-if="
                    hasDefaultForm ||
                    availableForms.length ===
                      0
                  "
                  value=""
                >
                  Default / Original
                </option>

                <option
                  v-for="
                    formOption
                    in availableForms
                  "
                  :key="
                    formOption
                  "
                  :value="
                    formOption
                  "
                >
                  {{ formOption }}
                </option>
              </select>

              <input
                v-else
                v-model="form"
                type="text"
                placeholder="Optional"
              />
            </div>

            <div
              class="form-group alpha-group"
            >
              <label>
                <input
                  v-model="isAlpha"
                  type="checkbox"
                />

                Alpha Pokémon
              </label>
            </div>
          </div>

          <div class="marks-area">
            <h3>
              Marks & Ribbons
            </h3>

            <div class="marks-section">
              <h4>Marks</h4>

              <div class="marks-list">
                <label
                  v-for="
                    markOption
                    in availableMarks
                  "
                  :key="
                    markOption.name
                  "
                  class="mark-option"
                >
                  <input
                    v-model="
                      selectedMarks
                    "
                    type="checkbox"
                    :value="
                      markOption.name
                    "
                  />

                  <img
                    :src="
                      markOption.image
                    "
                    :alt="
                      markOption.name
                    "
                  />

                  <span>
                    {{
                      markOption.name
                    }}
                  </span>
                </label>
              </div>
            </div>

            <div class="marks-section">
              <h4>Ribbons</h4>

              <div class="marks-list">
                <label
                  v-for="
                    ribbonOption
                    in availableRibbons
                  "
                  :key="
                    ribbonOption.name
                  "
                  class="mark-option"
                >
                  <input
                    v-model="
                      selectedMarks
                    "
                    type="checkbox"
                    :value="
                      ribbonOption.name
                    "
                  />

                  <img
                    :src="
                      ribbonOption.image
                    "
                    :alt="
                      ribbonOption.name
                    "
                  />

                  <span>
                    {{
                      ribbonOption.name
                    }}
                  </span>
                </label>
              </div>
            </div>
          </div>
        </div>

        <!-- STEP 4 -->
        <div
          v-if="currentStep === 4"
          class="step-content"
        >
          <div class="section-heading">
            <span>Step 4</span>

            <h2>
              What happens next?
            </h2>

            <p>
              The shiny will be added to
              your collection and this
              phase will be saved.
            </p>
          </div>

          <div class="final-summary">
            <div>
              <span>
                Found
              </span>

              <strong>
                {{ pokemon }}
              </strong>
            </div>

            <div>
              <span>
                Result
              </span>

              <strong>
                {{
                  wasTarget
                    ? 'Target'
                    : 'Phase'
                }}
              </strong>
            </div>

            <div>
              <span>
                Game
              </span>

              <strong>
                {{
                  gameDisplayName(
                    selectedGame
                  )
                }}
              </strong>
            </div>

            <div
              v-if="hunt.usesCounter"
            >
              <span>
                Phase Encounters
              </span>

              <strong>
                {{
                  hunt.currentEncounters
                    .toLocaleString()
                }}
              </strong>
            </div>
          </div>

          <div class="finish-options">
            <article class="finish-card">
              <div>
                <h3>
                  Continue Hunt
                </h3>

                <p>
                  Save this phase,
                  reset Current
                  Encounters to 0 and
                  start Phase
                  {{
                    hunt.currentPhaseNumber +
                    1
                  }}.
                </p>

                <p class="keep-total">
                  Total encounters stay
                  at
                  {{
                    hunt.totalEncounters
                      .toLocaleString()
                  }}.
                </p>
              </div>

              <button
                type="button"
                class="primary-button"
                :disabled="
                  isSubmitting
                "
                @click="
                  saveShinyFound(true)
                "
              >
                Continue Hunt
              </button>
            </article>

            <article class="finish-card target-finish">
              <div>
                <h3>
                  Complete Hunt
                </h3>

                <p>
                  Save this phase and
                  mark the hunt as
                  completed.
                </p>
              </div>

              <button
                type="button"
                class="complete-button"
                :disabled="
                  isSubmitting
                "
                @click="
                  saveShinyFound(false)
                "
              >
                Complete Hunt
              </button>
            </article>
          </div>

          <p
            v-if="isSubmitting"
            class="saving"
          >
            Saving shiny...
          </p>
        </div>

        <footer class="form-actions">
          <button
            v-if="
              currentStep > 1
            "
            type="button"
            class="secondary-button"
            :disabled="
              isSubmitting
            "
            @click="
              previousStep
            "
          >
            ← Back
          </button>

          <div
            v-else
            class="spacer"
          ></div>

          <button
            v-if="
              currentStep < 4
            "
            type="button"
            class="primary-button"
            @click="nextStep"
          >
            <template
              v-if="
                currentStep === 1
              "
            >
              Next: Pokémon →
            </template>

            <template
              v-else-if="
                currentStep === 2
              "
            >
              Next: Details →
            </template>

            <template v-else>
              Next: Finish →
            </template>
          </button>
        </footer>
      </section>

      <datalist id="all-pokemon">
        <option
          v-for="
            entry
            in pokemonDex
          "
          :key="
            `${entry.dexNumber}-${entry.name}-${entry.form || ''}-${entry.gender || ''}`
          "
          :value="entry.name"
        />
      </datalist>
    </template>
  </main>
</template>

<style scoped>
.found-page {
  max-width: 1000px;
  margin: 0 auto;
  padding: 50px 20px;
}

.loading {
  text-align: center;
  font-weight: 700;
}

.page-header {
  margin-bottom: 30px;
}

.back-button {
  padding: 0;
  margin-bottom: 14px;

  border: none;
  background: none;

  color: #6b7280;
  font: inherit;
  font-weight: 700;

  cursor: pointer;
}

.page-header h1 {
  margin: 0;
  font-size: 2.4rem;
}

.page-header p {
  margin: 7px 0 0;
  color: #6b7280;
}

.steps {
  display: flex;
  align-items: center;

  margin-bottom: 28px;

  overflow-x: auto;
}

.step-wrapper {
  display: flex;
  align-items: center;
}

.step {
  display: flex;
  align-items: center;
  gap: 7px;

  color: #9ca3af;

  font-size: 0.85rem;
  font-weight: 700;

  white-space: nowrap;
}

.step span {
  display: flex;
  align-items: center;
  justify-content: center;

  width: 28px;
  height: 28px;

  border-radius: 50%;

  background: #e5e7eb;
}

.step.active {
  color: #111827;
}

.step.active span {
  background: #111827;
  color: white;
}

.step.complete {
  color: #166534;
}

.step.complete span {
  background: #dcfce7;
  color: #166534;
}

.step-line {
  width: 45px;
  height: 2px;

  margin: 0 10px;

  background: #e5e7eb;
}

.card {
  padding: 30px;

  border: 1px solid #e5e7eb;
  border-radius: 20px;

  background: white;

  box-shadow:
    0 6px 20px
    rgba(0, 0, 0, 0.06);
}

.step-content {
  display: flex;
  flex-direction: column;
  gap: 25px;
}

.section-heading > span {
  color: #6b7280;

  font-size: 0.75rem;
  font-weight: 800;

  text-transform: uppercase;
}

.section-heading h2 {
  margin: 5px 0 7px;
}

.section-heading p {
  margin: 0;
  color: #6b7280;
}

.error-message {
  padding: 12px 14px;
  margin-bottom: 20px;

  border-radius: 10px;

  background: #fee2e2;
  color: #991b1b;

  font-weight: 700;
}

.result-options,
.option-grid,
.pokemon-grid,
.finish-options {
  display: grid;
  grid-template-columns:
    repeat(2, 1fr);
  gap: 14px;
}

.result-card,
.select-card,
.pokemon-card {
  display: flex;
  flex-direction: column;
  gap: 6px;

  padding: 18px;

  border: 2px solid #e5e7eb;
  border-radius: 14px;

  background: white;

  text-align: left;

  cursor: pointer;
}

.result-card.selected,
.select-card.selected,
.pokemon-card.selected {
  border-color: #111827;
  background: #f9fafb;
}

.result-card strong {
  font-size: 1.15rem;
}

.result-card span,
.select-card span,
.pokemon-card span {
  color: #6b7280;
  font-size: 0.82rem;
}

.target-result.selected {
  border-color: #166534;
  background: #f0fdf4;
}

.target-preview {
  padding: 15px;

  border-radius: 12px;

  background: #f9fafb;
}

.target-preview > span {
  display: block;

  margin-bottom: 9px;

  color: #6b7280;
  font-size: 0.8rem;
  font-weight: 700;
}

.preview-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
}

.preview-tags strong {
  padding: 5px 9px;

  border-radius: 999px;

  background: white;

  font-size: 0.8rem;
}

.section-block h3 {
  margin: 0 0 10px;
}

.help-text {
  margin: -4px 0 12px;

  color: #6b7280;
  font-size: 0.85rem;
}

.single-game,
.selected-pokemon,
.found-summary {
  padding: 14px 16px;

  border-radius: 12px;

  background: #f9fafb;
}

.single-game strong {
  margin-left: 4px;
}

.manual-pokemon {
  display: flex;
  flex-direction: column;
  gap: 7px;
}

.manual-pokemon label {
  font-weight: 700;
}

.manual-pokemon input,
.form-group input,
.form-group select {
  width: 100%;
  box-sizing: border-box;

  padding: 11px 12px;

  border: 1px solid #d1d5db;
  border-radius: 10px;

  background: white;

  font: inherit;
}

.outline-button,
.secondary-button {
  padding: 10px 15px;

  border: 1px solid #d1d5db;
  border-radius: 10px;

  background: white;

  color: #374151;

  font-weight: 700;
  cursor: pointer;
}

.found-summary {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.found-summary strong {
  font-size: 1.2rem;
}

.found-summary span {
  color: #6b7280;
  font-size: 0.85rem;
}

.form-grid {
  display: grid;
  grid-template-columns:
    repeat(2, 1fr);
  gap: 18px;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 7px;
}

.form-group label {
  font-weight: 700;
}

.alpha-group {
  justify-content: center;
}

.alpha-group label {
  display: flex;
  align-items: center;
  gap: 9px;

  cursor: pointer;
}

.alpha-group input {
  width: auto;
}

.marks-area {
  padding-top: 8px;
}

.marks-area > h3 {
  margin-top: 0;
}

.marks-section + .marks-section {
  margin-top: 20px;
}

.marks-section h4 {
  margin: 0 0 9px;
}

.marks-list {
  display: grid;
  grid-template-columns:
    repeat(3, 1fr);
  gap: 8px;

  max-height: 250px;
  overflow-y: auto;

  padding: 10px;

  border: 1px solid #e5e7eb;
  border-radius: 12px;

  background: #f9fafb;
}

.mark-option {
  display: flex;
  align-items: center;
  gap: 7px;

  padding: 7px;

  border-radius: 8px;

  background: white;

  font-size: 0.82rem;
  cursor: pointer;
}

.mark-option input {
  width: auto;
}

.mark-option img {
  width: 28px;
  height: 28px;
  object-fit: contain;
}

.final-summary {
  display: grid;
  grid-template-columns:
    repeat(4, 1fr);
  gap: 10px;
}

.final-summary div {
  padding: 14px;

  border-radius: 12px;

  background: #f9fafb;
}

.final-summary span {
  display: block;

  margin-bottom: 4px;

  color: #6b7280;
  font-size: 0.75rem;
}

.final-summary strong {
  font-size: 0.9rem;
}

.finish-card {
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  gap: 18px;

  padding: 20px;

  border: 2px solid #e5e7eb;
  border-radius: 15px;
}

.finish-card h3 {
  margin: 0 0 7px;
}

.finish-card p {
  margin: 0;

  color: #6b7280;
  line-height: 1.5;
}

.keep-total {
  margin-top: 10px !important;
  font-size: 0.82rem;
}

.target-finish {
  border-color: #bbf7d0;
}

.primary-button,
.complete-button {
  padding: 11px 18px;

  border: none;
  border-radius: 10px;

  color: white;

  font-weight: 800;
  cursor: pointer;
}

.primary-button {
  background: #111827;
}

.complete-button {
  background: #166534;
}

.primary-button:disabled,
.complete-button:disabled,
.secondary-button:disabled {
  opacity: 0.5;
  cursor: default;
}

.saving {
  margin: 0;

  text-align: center;
  color: #6b7280;
}

.form-actions {
  display: flex;
  justify-content: space-between;
  align-items: center;

  margin-top: 30px;
  padding-top: 22px;

  border-top: 1px solid #e5e7eb;
}

.spacer {
  width: 1px;
}

@media (max-width: 750px) {
  .found-page {
    padding: 30px 16px;
  }

  .page-header h1 {
    font-size: 2rem;
  }

  .card {
    padding: 20px;
  }

  .result-options,
  .option-grid,
  .pokemon-grid,
  .finish-options,
  .form-grid,
  .final-summary {
    grid-template-columns: 1fr;
  }

  .marks-list {
    grid-template-columns: 1fr;
  }
}
</style>