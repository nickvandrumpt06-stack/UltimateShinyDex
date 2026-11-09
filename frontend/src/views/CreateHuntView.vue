<script setup>
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'

import {
  games as gameOptions,
  natures as natureOptions
} from '../data/options'

import { pokemonDex } from '../data/pokedex'

const router = useRouter()

const currentStep = ref(1)

const huntName = ref('')
const usesCounter = ref(true)
const counterStep = ref(1)

const gameCount = ref(1)

const errorMessage = ref('')
const isSubmitting = ref(false)

const gameInstances = ref([
  createGameInstance()
])

const targets = ref([
  createTarget()
])

function createGameInstance() {
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
      pokemonDex.map((entry) => entry.name)
    )
  ].sort((a, b) =>
    a.localeCompare(b)
  )
})

const encounteredPokemonNames = computed(() => {
  const names = []

  for (const game of gameInstances.value) {
    for (const encounter of game.encounters) {
      if (
        encounter.pokemon &&
        !names.includes(encounter.pokemon)
      ) {
        names.push(encounter.pokemon)
      }
    }
  }

  return names.sort((a, b) =>
    a.localeCompare(b)
  )
})

const stepTitle = computed(() => {
  if (currentStep.value === 1) {
    return 'Hunt Basics'
  }

  if (currentStep.value === 2) {
    return 'Games'
  }

  if (currentStep.value === 3) {
    return 'Encounter Pools'
  }

  if (currentStep.value === 4) {
    return 'Targets'
  }

  if (currentStep.value === 5) {
    return 'Review Hunt'
  }

  return ''
})

function updateGameCount() {
  let wantedCount =
    Number(gameCount.value)

  if (
    !wantedCount ||
    wantedCount < 1
  ) {
    wantedCount = 1
    gameCount.value = 1
  }

  if (wantedCount > 20) {
    wantedCount = 20
    gameCount.value = 20
  }

  while (
    gameInstances.value.length <
    wantedCount
  ) {
    gameInstances.value.push(
      createGameInstance()
    )
  }

  while (
    gameInstances.value.length >
    wantedCount
  ) {
    gameInstances.value.pop()
  }

  applySuggestedLabels()
}

function getDuplicateNumber(index) {
  const gameName =
    gameInstances.value[index].game

  if (!gameName) {
    return null
  }

  const sameGames =
    gameInstances.value.filter(
      (instance) =>
        instance.game === gameName
    )

  if (sameGames.length <= 1) {
    return null
  }

  let number = 0

  for (
    let i = 0;
    i <= index;
    i++
  ) {
    if (
      gameInstances.value[i].game ===
      gameName
    ) {
      number++
    }
  }

  return number
}

function suggestedLabel(index) {
  const instance =
    gameInstances.value[index]

  if (!instance.game) {
    return ''
  }

  const duplicateNumber =
    getDuplicateNumber(index)

  if (!duplicateNumber) {
    return instance.game
  }

  return `${instance.game} #${duplicateNumber}`
}

function applySuggestedLabels() {
  gameInstances.value.forEach(
    (instance, index) => {
      const shouldReplaceLabel =
        !instance.label ||
        instance.label === instance.game ||
        /^.+ #\d+$/.test(
          instance.label
        )

      if (shouldReplaceLabel) {
        instance.label =
          suggestedLabel(index)
      }
    }
  )
}

function onGameChanged() {
  applySuggestedLabels()
}

function getGameDisplayName(index) {
  return (
    gameInstances.value[index].label ||
    suggestedLabel(index) ||
    `Game ${index + 1}`
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
  targetGameIndex,
  sourceGameIndex
) {
  if (sourceGameIndex === '') {
    return
  }

  const source =
    gameInstances.value[
      Number(sourceGameIndex)
    ]

  gameInstances.value[
    targetGameIndex
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

function hasEncounterPercentages(game) {
  return game.encounters.some(
    (encounter) =>
      encounter.encounterPercentage !==
        null &&
      encounter.encounterPercentage !==
        ''
  )
}

function allEncounterPercentagesFilled(
  game
) {
  return game.encounters.every(
    (encounter) =>
      encounter.encounterPercentage !==
        null &&
      encounter.encounterPercentage !==
        ''
  )
}

function encounterTotalIsValid(game) {
  if (
    !hasEncounterPercentages(game)
  ) {
    return true
  }

  const total =
    encounterTotal(game)

  return (
    Math.abs(
      100 - total
    ) <= 0.11
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
    return
  }

  targets.value.splice(
    index,
    1
  )
}

function validateStepOne() {
  if (!huntName.value.trim()) {
    errorMessage.value =
      'Please give your hunt a name.'

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

  return true
}

function validateStepTwo() {
  if (
    gameInstances.value.length < 1
  ) {
    errorMessage.value =
      'A hunt needs at least one game instance.'

    return false
  }

  const missingGame =
    gameInstances.value.some(
      (instance) =>
        !instance.game
    )

  if (missingGame) {
    errorMessage.value =
      'Please select a game for every game instance.'

    return false
  }

  const invalidOdds =
    gameInstances.value.some(
      (instance) =>
        instance.shinyOdds !== null &&
        instance.shinyOdds !== '' &&
        Number(
          instance.shinyOdds
        ) < 1
    )

  if (invalidOdds) {
    errorMessage.value =
      'Shiny odds must be at least 1.'

    return false
  }

  return true
}

function validateStepThree() {
  for (
    const game
    of gameInstances.value
  ) {
    for (
      const encounter
      of game.encounters
    ) {
      if (
        !encounter.pokemon.trim()
      ) {
        errorMessage.value =
          `${game.label || game.game}: every encounter row needs a Pokémon.`

        return false
      }

      if (
        encounter.encounterPercentage !==
          null &&
        encounter.encounterPercentage !==
          '' &&
        (
          Number(
            encounter.encounterPercentage
          ) < 0 ||
          Number(
            encounter.encounterPercentage
          ) > 100
        )
      ) {
        errorMessage.value =
          `${game.label || game.game}: encounter percentages must be between 0 and 100%.`

        return false
      }
    }

    if (
      hasEncounterPercentages(game)
    ) {
      if (
        !allEncounterPercentagesFilled(
          game
        )
      ) {
        errorMessage.value =
          `${game.label || game.game}: if you use encounter percentages, every encounter needs a percentage.`

        return false
      }

      if (
        !encounterTotalIsValid(
          game
        )
      ) {
        errorMessage.value =
          `${game.label || game.game}: encounter percentages currently total ${encounterTotal(game)}%. They should total 100%.`

        return false
      }
    }
  }

  return true
}

function validateStepFour() {
  if (
    targets.value.length < 1
  ) {
    errorMessage.value =
      'Please add at least one target.'

    return false
  }

  const missingPokemon =
    targets.value.some(
      (target) =>
        !target.pokemon.trim()
    )

  if (missingPokemon) {
    errorMessage.value =
      'Every target needs a Pokémon.'

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
    currentStep.value === 4 &&
    !validateStepFour()
  ) {
    return
  }

  if (
    currentStep.value < 5
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
  router.push('/hunts')
}

function normalizedTarget(target) {
  return {
    pokemon:
      target.pokemon.trim(),

    form:
      target.form.trim()
        ? target.form.trim()
        : null,

    gender:
      target.gender ||
      null,

    nature:
      target.nature ||
      null,

    isAlpha:
      target.isAlpha,

    mark:
      target.mark.trim()
        ? target.mark.trim()
        : null
  }
}

async function createHunt() {
  errorMessage.value = ''

  if (
    !validateStepOne() ||
    !validateStepTwo() ||
    !validateStepThree() ||
    !validateStepFour()
  ) {
    return
  }

  isSubmitting.value = true

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

    games:
      gameInstances.value.map(
        (instance) => ({
          game:
            instance.game,

          label:
            instance.label.trim()
              ? instance.label.trim()
              : null,

          shinyOdds:
            instance.shinyOdds ===
              null ||
            instance.shinyOdds ===
              ''
              ? null
              : Number(
                  instance.shinyOdds
                ),

          encounters:
            instance.encounters.map(
              (encounter) => ({
                pokemon:
                  encounter.pokemon.trim(),

                form:
                  encounter.form.trim()
                    ? encounter.form.trim()
                    : null,

                encounterPercentage:
                  encounter.encounterPercentage ===
                    null ||
                  encounter.encounterPercentage ===
                    ''
                    ? null
                    : Number(
                        encounter.encounterPercentage
                      )
              })
            )
        })
      ),

    targets:
      targets.value.map(
        normalizedTarget
      )
  }

  try {
    const response =
      await fetch(
        'http://localhost:5119/api/hunts',
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
      const responseText =
        await response.text()

      console.error(
        responseText
      )

      throw new Error(
        'Could not create hunt'
      )
    }

    const createdHunt =
      await response.json()

    router.push(
      `/hunts/${createdHunt.id}`
    )
  } catch (error) {
    console.error(error)

    errorMessage.value =
      'Something went wrong while creating the hunt.'
  } finally {
    isSubmitting.value = false
  }
}
</script>

<template>
  <main class="create-hunt-page">
    <header class="page-header">
      <div>
        <h1>Create Hunt</h1>

        <p>
          Set up your shiny hunt before
          you start hunting.
        </p>
      </div>

      <button
        type="button"
        class="cancel-button"
        @click="cancel"
      >
        Cancel
      </button>
    </header>

    <section class="step-indicator">
      <div
        v-for="step in 5"
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
              'Basics',
              'Games',
              'Encounters',
              'Targets',
              'Review'
            ][step - 1]
          }}
        </div>

        <div
          v-if="step < 5"
          class="step-line"
        ></div>
      </div>
    </section>

    <section class="form-card">
      <div class="form-heading">
        <span class="step-label">
          Step {{ currentStep }}
        </span>

        <h2>
          {{ stepTitle }}
        </h2>
      </div>

      <p
        v-if="errorMessage"
        class="error-message"
      >
        {{ errorMessage }}
      </p>

      <!-- STEP 1 -->
      <div
        v-if="currentStep === 1"
        class="form-content"
      >
        <div class="field">
          <label>
            Hunt Name
          </label>

          <input
            v-model="huntName"
            type="text"
            placeholder="e.g. Route 5 Meowth Hunt"
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
                Track current and
                total encounters.
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
                Track phases without
                counting encounters.
              </span>
            </button>
          </div>
        </div>

        <div
          v-if="usesCounter"
          class="field"
        >
          <label>
            Default Counter Step
          </label>

          <input
            v-model.number="
              counterStep
            "
            type="number"
            min="1"
          />

          <p class="field-help">
            The hunt page will start
            with +{{ counterStep || 1 }}
            and -{{ counterStep || 1 }}.
          </p>
        </div>
      </div>

      <!-- STEP 2 -->
      <div
        v-if="currentStep === 2"
        class="form-content"
      >
        <div class="field game-count-field">
          <label>
            Number of Game Instances
          </label>

          <input
            v-model.number="
              gameCount
            "
            type="number"
            min="1"
            max="20"
            @input="
              updateGameCount
            "
          />

          <p class="field-help">
            Separate copies count
            separately. For example,
            2 HeartGold +
            2 SoulSilver = 4 game
            instances.
          </p>
        </div>

        <div class="game-list">
          <article
            v-for="
              (
                instance,
                index
              ) in gameInstances
            "
            :key="index"
            class="game-card"
          >
            <div class="game-card-header">
              <span class="game-number">
                Game {{ index + 1 }}
              </span>

              <h3>
                {{
                  instance.label ||
                  suggestedLabel(index) ||
                  'Choose Game'
                }}
              </h3>
            </div>

            <div class="game-fields">
              <div class="field">
                <label>
                  Game
                </label>

                <select
                  v-model="
                    instance.game
                  "
                  @change="
                    onGameChanged
                  "
                >
                  <option
                    value=""
                    disabled
                  >
                    Choose a game
                  </option>

                  <option
                    v-for="
                      game
                      in gameOptions
                    "
                    :key="game"
                    :value="game"
                  >
                    {{ game }}
                  </option>
                </select>
              </div>

              <div class="field">
                <label>
                  Label
                </label>

                <input
                  v-model="
                    instance.label
                  "
                  type="text"
                  :placeholder="
                    suggestedLabel(
                      index
                    )
                  "
                />

                <p class="field-help">
                  Useful when using
                  several copies of
                  the same game.
                </p>
              </div>

              <div class="field">
                <label>
                  Shiny Odds
                </label>

                <div class="odds-input">
                  <span>
                    1 /
                  </span>

                  <input
                    v-model.number="
                      instance.shinyOdds
                    "
                    type="number"
                    min="1"
                  />
                </div>
              </div>
            </div>
          </article>
        </div>
      </div>

      <!-- STEP 3 -->
      <div
        v-if="currentStep === 3"
        class="form-content"
      >
        <p class="section-description">
          Encounter pools are optional.
          Add the Pokémon that can
          appear in each game and,
          optionally, their encounter
          percentages.
        </p>

        <article
          v-for="
            (
              instance,
              gameIndex
            ) in gameInstances
          "
          :key="gameIndex"
          class="encounter-card"
        >
          <div class="encounter-header">
            <div>
              <span class="game-number">
                Game
                {{ gameIndex + 1 }}
              </span>

              <h3>
                {{
                  getGameDisplayName(
                    gameIndex
                  )
                }}
              </h3>
            </div>

            <div
              v-if="
                gameInstances.length > 1
              "
              class="copy-area"
            >
              <label>
                Copy pool from
              </label>

              <select
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
                  Choose game...
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
                    getGameDisplayName(
                      sourceIndex
                    )
                  }}
                </option>
              </select>
            </div>
          </div>

          <div
            v-if="
              instance.encounters
                .length === 0
            "
            class="empty-pool"
          >
            No encounter pool added.
            You can leave this empty.
          </div>

          <div
            v-else
            class="encounter-list"
          >
            <div
              v-for="
                (
                  encounter,
                  encounterIndex
                ) in instance.encounters
              "
              :key="
                encounterIndex
              "
              class="encounter-row"
            >
              <div class="field">
                <label>
                  Pokémon
                </label>

                <input
                  v-model="
                    encounter.pokemon
                  "
                  type="text"
                  list="pokemon-list"
                  placeholder="Meowth"
                />
              </div>

              <div class="field">
                <label>
                  Form
                </label>

                <input
                  v-model="
                    encounter.form
                  "
                  type="text"
                  placeholder="Optional"
                />
              </div>

              <div class="field">
                <label>
                  Encounter %
                </label>

                <div class="percentage-input">
                  <input
                    v-model.number="
                      encounter.encounterPercentage
                    "
                    type="number"
                    min="0"
                    max="100"
                    step="0.01"
                    placeholder="Optional"
                  />

                  <span>%</span>
                </div>
              </div>

              <button
                type="button"
                class="remove-button"
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
          </div>

          <div class="pool-footer">
            <button
              type="button"
              class="outline-button"
              @click="
                addEncounter(
                  gameIndex
                )
              "
            >
              + Add Encounter
            </button>

            <span
              v-if="
                hasEncounterPercentages(
                  instance
                )
              "
              class="percentage-total"
              :class="{
                warning:
                  !encounterTotalIsValid(
                    instance
                  )
              }"
            >
              Total:
              {{
                encounterTotal(
                  instance
                )
              }}%
            </span>
          </div>
        </article>
      </div>

      <!-- STEP 4 -->
      <div
        v-if="currentStep === 4"
        class="form-content"
      >
        <div>
          <p class="section-description">
            Choose one or more targets.
            Only the Pokémon itself is
            required. Everything else
            can be left as Any.
          </p>

          <p
            v-if="
              encounteredPokemonNames.length
            "
            class="field-help target-help"
          >
            Pokémon from your encounter
            pools are available below,
            but you can also type any
            other Pokémon.
          </p>
        </div>

        <article
          v-for="
            (
              target,
              index
            ) in targets
          "
          :key="index"
          class="target-card"
        >
          <div class="target-header">
            <div>
              <span class="game-number">
                Target {{ index + 1 }}
              </span>

              <h3>
                {{
                  target.pokemon ||
                  'Choose Target'
                }}
              </h3>
            </div>

            <button
              v-if="
                targets.length > 1
              "
              type="button"
              class="remove-target-button"
              @click="
                removeTarget(index)
              "
            >
              Remove
            </button>
          </div>

          <div class="target-grid">
            <div class="field">
              <label>
                Pokémon
              </label>

              <input
                v-model="
                  target.pokemon
                "
                type="text"
                list="pokemon-list"
                placeholder="Meowth"
              />
            </div>

            <div class="field">
              <label>
                Form
              </label>

              <input
                v-model="
                  target.form
                "
                type="text"
                placeholder="Any"
              />
            </div>

            <div class="field">
              <label>
                Gender
              </label>

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
              <label>
                Nature
              </label>

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
              <label>
                Alpha Status
              </label>

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
              <label>
                Mark
              </label>

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

        <button
          type="button"
          class="outline-button"
          @click="addTarget"
        >
          + Add Target
        </button>
      </div>

      <!-- STEP 5 -->
      <div
        v-if="currentStep === 5"
        class="form-content"
      >
        <section class="review-section">
          <h3>Basics</h3>

          <div class="review-row">
            <span>Hunt</span>

            <strong>
              {{ huntName }}
            </strong>
          </div>

          <div class="review-row">
            <span>Counter</span>

            <strong>
              {{
                usesCounter
                  ? `Yes (+${counterStep})`
                  : 'No counter'
              }}
            </strong>
          </div>
        </section>

        <section class="review-section">
          <h3>Games</h3>

          <div
            v-for="
              (
                game,
                index
              ) in gameInstances
            "
            :key="index"
            class="review-game"
          >
            <strong>
              {{
                getGameDisplayName(
                  index
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

              •
              {{
                game.encounters.length
              }}
              encounters
            </span>
          </div>
        </section>

        <section class="review-section">
          <h3>Targets</h3>

          <div
            v-for="
              (
                target,
                index
              ) in targets
            "
            :key="index"
            class="review-target"
          >
            <strong>
              {{ target.pokemon }}
            </strong>

            <span>
              {{
                [
                  target.form,
                  target.gender,
                  target.nature,
                  target.isAlpha === true
                    ? 'Alpha'
                    : target.isAlpha === false
                      ? 'Not Alpha'
                      : null,
                  target.mark
                    ? `${target.mark} Mark`
                    : null
                ]
                  .filter(Boolean)
                  .join(' • ') ||
                'Any variation'
              }}
            </span>
          </div>
        </section>

        <p class="review-ready">
          Ready to start hunting.
        </p>
      </div>

      <datalist id="pokemon-list">
        <option
          v-for="
            pokemon in pokemonNames
          "
          :key="pokemon"
          :value="pokemon"
        />
      </datalist>

      <footer class="form-actions">
        <button
          v-if="
            currentStep > 1
          "
          type="button"
          class="secondary-button"
          @click="
            previousStep
          "
        >
          ← Back
        </button>

        <div
          v-else
          class="action-spacer"
        ></div>

        <button
          v-if="
            currentStep < 5
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
            Next: Games →
          </template>

          <template
            v-else-if="
              currentStep === 2
            "
          >
            Next: Encounter Pools →
          </template>

          <template
            v-else-if="
              currentStep === 3
            "
          >
            Next: Targets →
          </template>

          <template
            v-else
          >
            Next: Review →
          </template>
        </button>

        <button
          v-else
          type="button"
          class="primary-button"
          :disabled="
            isSubmitting
          "
          @click="
            createHunt
          "
        >
          {{
            isSubmitting
              ? 'Creating Hunt...'
              : 'Create Hunt'
          }}
        </button>
      </footer>
    </section>
  </main>
</template>

<style scoped>
.create-hunt-page {
  max-width: 1000px;
  margin: 0 auto;
  padding: 50px 20px;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 20px;

  margin-bottom: 35px;
}

.page-header h1 {
  margin: 0;
  font-size: 2.4rem;
}

.page-header p {
  margin: 8px 0 0;
  color: #6b7280;
}

.cancel-button,
.secondary-button {
  padding: 10px 16px;

  border: 1px solid #d1d5db;
  border-radius: 10px;

  background: white;
  color: #374151;

  font-weight: 700;
  cursor: pointer;
}

.step-indicator {
  display: flex;
  align-items: center;

  margin-bottom: 30px;

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
  color: #6b7280;
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
  min-width: 30px;
  height: 2px;

  margin: 0 10px;

  background: #e5e7eb;
}

.form-card {
  padding: 30px;

  border: 1px solid #e5e7eb;
  border-radius: 20px;

  background: white;

  box-shadow:
    0 6px 20px
    rgba(0, 0, 0, 0.06);
}

.form-heading {
  margin-bottom: 25px;
}

.step-label {
  color: #6b7280;

  font-size: 0.8rem;
  font-weight: 700;

  text-transform: uppercase;
}

.form-heading h2 {
  margin: 5px 0 0;
}

.form-content {
  display: flex;
  flex-direction: column;
  gap: 26px;
}

.field {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.field label {
  font-weight: 700;
}

.field input,
.field select,
.copy-area select {
  width: 100%;
  box-sizing: border-box;

  padding: 11px 12px;

  border: 1px solid #d1d5db;
  border-radius: 10px;

  background: white;

  font: inherit;
}

.field-help,
.section-description {
  margin: 0;

  color: #6b7280;

  font-size: 0.85rem;
  line-height: 1.5;
}

.game-count-field {
  max-width: 350px;
}

.choice-grid {
  display: grid;
  grid-template-columns:
    1fr 1fr;
  gap: 12px;
}

.choice-card {
  padding: 18px;

  border:
    2px solid #e5e7eb;
  border-radius: 14px;

  background: white;

  text-align: left;
  cursor: pointer;
}

.choice-card.selected {
  border-color: #111827;
  background: #f9fafb;
}

.choice-card strong {
  display: block;
  margin-bottom: 5px;
}

.choice-card span {
  color: #6b7280;
  font-size: 0.85rem;
}

.game-list {
  display: flex;
  flex-direction: column;
  gap: 18px;
}

.game-card,
.encounter-card,
.target-card {
  padding: 20px;

  border: 1px solid #e5e7eb;
  border-radius: 16px;

  background: #f9fafb;
}

.game-card-header {
  margin-bottom: 18px;
}

.encounter-header,
.target-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-end;
  gap: 20px;

  margin-bottom: 18px;
}

.game-number {
  color: #6b7280;

  font-size: 0.75rem;
  font-weight: 700;

  text-transform: uppercase;
}

.game-card h3,
.encounter-card h3,
.target-card h3 {
  margin: 4px 0 0;
}

.game-fields {
  display: grid;
  grid-template-columns:
    minmax(0, 1fr)
    minmax(0, 1fr)
    160px;
  gap: 15px;
}

.odds-input,
.percentage-input {
  display: flex;
  align-items: center;
  gap: 8px;
}

.odds-input span,
.percentage-input span {
  font-weight: 700;
}

.copy-area {
  min-width: 220px;
}

.copy-area label {
  display: block;

  margin-bottom: 6px;

  font-size: 0.8rem;
  font-weight: 700;
}

.empty-pool {
  padding: 18px;

  border:
    1px dashed #d1d5db;
  border-radius: 12px;

  color: #6b7280;
  text-align: center;
}

.encounter-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.encounter-row {
  display: grid;
  grid-template-columns:
    minmax(0, 1fr)
    minmax(0, 0.8fr)
    150px
    42px;
  gap: 12px;

  align-items: end;
}

.remove-button {
  width: 42px;
  height: 42px;

  border: none;
  border-radius: 10px;

  background: #fee2e2;
  color: #991b1b;

  font-size: 1.4rem;
  font-weight: 700;

  cursor: pointer;
}

.pool-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 15px;

  margin-top: 16px;
}

.outline-button {
  align-self: flex-start;

  padding: 9px 14px;

  border:
    1px solid #d1d5db;
  border-radius: 10px;

  background: white;

  font-weight: 700;
  cursor: pointer;
}

.percentage-total {
  color: #166534;
  font-weight: 700;
}

.percentage-total.warning {
  color: #b45309;
}

.target-help {
  margin-top: 8px;
}

.target-grid {
  display: grid;
  grid-template-columns:
    repeat(2, minmax(0, 1fr));
  gap: 16px;
}

.remove-target-button {
  padding: 7px 12px;

  border: none;
  border-radius: 8px;

  background: #fee2e2;
  color: #991b1b;

  font-weight: 700;
  cursor: pointer;
}

.review-section {
  padding: 20px;

  border: 1px solid #e5e7eb;
  border-radius: 14px;

  background: #f9fafb;
}

.review-section h3 {
  margin-top: 0;
}

.review-row {
  display: flex;
  justify-content: space-between;
  gap: 20px;

  padding: 8px 0;
}

.review-game,
.review-target {
  display: flex;
  flex-direction: column;
  gap: 3px;

  padding: 9px 0;

  border-bottom:
    1px solid #e5e7eb;
}

.review-game:last-child,
.review-target:last-child {
  border-bottom: none;
}

.review-game span,
.review-target span,
.review-row span {
  color: #6b7280;
}

.review-ready {
  margin: 0;

  text-align: center;
  font-weight: 700;
}

.error-message {
  margin-bottom: 20px;
  padding: 11px 14px;

  border-radius: 10px;

  background: #fee2e2;
  color: #991b1b;

  font-weight: 600;
}

.form-actions {
  display: flex;
  justify-content: space-between;
  align-items: center;

  margin-top: 32px;
  padding-top: 22px;

  border-top:
    1px solid #e5e7eb;
}

.primary-button {
  padding: 11px 18px;

  border: none;
  border-radius: 10px;

  background: #111827;
  color: white;

  font-weight: 700;
  cursor: pointer;
}

.primary-button:disabled {
  opacity: 0.55;
  cursor: default;
}

.action-spacer {
  width: 1px;
}

@media (max-width: 750px) {
  .create-hunt-page {
    padding: 30px 16px;
  }

  .page-header,
  .encounter-header,
  .target-header {
    align-items: flex-start;
    flex-direction: column;
  }

  .page-header h1 {
    font-size: 2rem;
  }

  .form-card {
    padding: 20px;
  }

  .choice-grid,
  .game-fields,
  .encounter-row,
  .target-grid {
    grid-template-columns: 1fr;
  }

  .copy-area {
    width: 100%;
    min-width: 0;
  }

  .remove-button {
    width: 100%;
  }

  .pool-footer {
    align-items: flex-start;
    flex-direction: column;
  }

  .review-row {
    align-items: flex-start;
    flex-direction: column;
  }
}
</style>