<script setup>
import {
  computed,
  onMounted,
  ref
} from 'vue'

import {
  RouterLink,
  useRoute,
  useRouter
} from 'vue-router'

import { pokemonDex } from '../data/pokedex'

import {
  formSprites,
  shinyFemale
} from '../data/formSprites'

const route = useRoute()
const router = useRouter()

const hunt = ref(null)
const isLoading = ref(true)
const errorMessage = ref('')

const isUpdatingCounter = ref(false)
const isDeleting = ref(false)
const counterStepInput = ref(1)

function normalizeValue(value) {
  return (value ?? '')
    .trim()
    .toLowerCase()
}

async function deleteHunt() {
  if (!hunt.value || isDeleting.value) {
    return
  }

  const confirmed = window.confirm(
    `Delete "${hunt.value.name}"?\n\n` +
    'The hunt, encounter pools, targets and phase history will be deleted.\n' +
    'Shinies already added to My Shinies will NOT be deleted.'
  )

  if (!confirmed) {
    return
  }

  isDeleting.value = true

  try {
    const response = await fetch(
      `http://localhost:5119/api/hunts/${hunt.value.id}`,
      {
        method: 'DELETE'
      }
    )

    if (!response.ok) {
      throw new Error(
        'Could not delete hunt'
      )
    }

    router.push('/hunts')
  } catch (error) {
    console.error(error)

    alert(
      'Something went wrong while deleting the hunt.'
    )
  } finally {
    isDeleting.value = false
  }
}

function findDexEntry(
  pokemon,
  form = null,
  gender = null
) {
  const normalizedPokemon =
    normalizeValue(pokemon)

  const normalizedForm =
    normalizeValue(form)

  const normalizedGender =
    normalizeValue(gender)

  return (
    pokemonDex.find(
      (entry) =>
        normalizeValue(entry.name) ===
          normalizedPokemon &&
        normalizeValue(entry.form) ===
          normalizedForm &&
        normalizeValue(entry.gender) ===
          normalizedGender
    ) ||

    pokemonDex.find(
      (entry) =>
        normalizeValue(entry.name) ===
          normalizedPokemon &&
        normalizeValue(entry.form) ===
          normalizedForm
    ) ||

    pokemonDex.find(
      (entry) =>
        normalizeValue(entry.name) ===
          normalizedPokemon &&
        normalizeValue(entry.gender) ===
          normalizedGender
    ) ||

    pokemonDex.find(
      (entry) =>
        normalizeValue(entry.name) ===
          normalizedPokemon &&
        !entry.form
    ) ||

    pokemonDex.find(
      (entry) =>
        normalizeValue(entry.name) ===
          normalizedPokemon
    ) ||

    null
  )
}

function getPokemonSprite({
  pokemon,
  form = null,
  gender = null,
  customSpritePath = null
}) {
  if (customSpritePath) {
    return `http://localhost:5119${customSpritePath}`
  }

  const dexEntry =
    findDexEntry(
      pokemon,
      form,
      gender
    )

  if (!dexEntry?.dexNumber) {
    return null
  }

  const genderSpriteKey =
    `${pokemon}|${form ?? ''}|${gender ?? ''}`

  if (formSprites[genderSpriteKey]) {
    return formSprites[genderSpriteKey]
  }

  const formSpriteKey =
    `${pokemon}|${form ?? ''}`

  if (formSprites[formSpriteKey]) {
    return formSprites[formSpriteKey]
  }

  if (gender === 'Female') {
    return shinyFemale(
      dexEntry.dexNumber
    )
  }

  return `https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/shiny/${dexEntry.dexNumber}.png`
}

function getTargetSprite(target) {
  return getPokemonSprite({
    pokemon: target.pokemon,
    form: target.form,
    gender: target.gender
  })
}

function getEncounterSprite(encounter) {
  return getPokemonSprite({
    pokemon: encounter.pokemon,
    form: encounter.form
  })
}

function getPhaseSprite(phase) {
  if (!phase.foundShiny) {
    return null
  }

  return getPokemonSprite({
    pokemon:
      phase.foundShiny.pokemon,

    form:
      phase.foundShiny.form,

    gender:
      phase.foundShiny.gender,

    customSpritePath:
      phase.foundShiny.customSpritePath
  })
}

function getTargetSubtitle(target) {
  return [
    target.form,
    target.gender
  ]
    .filter(Boolean)
    .join(' • ')
}

function getEncounterSubtitle(
  encounter
) {
  return [
    encounter.form
  ]
    .filter(Boolean)
    .join(' • ')
}

function getPhaseSubtitle(phase) {
  if (!phase.foundShiny) {
    return ''
  }

  return [
    phase.foundShiny.form,
    phase.foundShiny.gender
  ]
    .filter(Boolean)
    .join(' • ')
}

async function loadHunt() {
  try {
    const response = await fetch(
      `http://localhost:5119/api/hunts/${route.params.id}`
    )

    if (!response.ok) {
      if (
        response.status === 404
      ) {
        throw new Error(
          'Hunt not found'
        )
      }

      throw new Error(
        'Could not load hunt'
      )
    }

    hunt.value =
      await response.json()

    counterStepInput.value =
      hunt.value.counterStep
  } catch (error) {
    console.error(error)

    errorMessage.value =
      error.message ===
      'Hunt not found'
        ? 'Hunt not found.'
        : 'Could not load this hunt.'
  } finally {
    isLoading.value = false
  }
}

async function adjustCounter(amount) {
  if (
    !hunt.value ||
    isUpdatingCounter.value
  ) {
    return
  }

  isUpdatingCounter.value = true

  try {
    const response =
      await fetch(
        `http://localhost:5119/api/hunts/${hunt.value.id}/counter`,
        {
          method: 'POST',

          headers: {
            'Content-Type':
              'application/json'
          },

          body: JSON.stringify({
            amount
          })
        }
      )

    if (!response.ok) {
      throw new Error(
        'Could not update counter'
      )
    }

    const result =
      await response.json()

    hunt.value.currentEncounters =
      result.currentEncounters

    hunt.value.totalEncounters =
      result.totalEncounters
  } catch (error) {
    console.error(error)

    alert(
      'Something went wrong while updating the counter.'
    )
  } finally {
    isUpdatingCounter.value = false
  }
}

async function saveCounterStep() {
  const step =
    Number(counterStepInput.value)

  if (step < 1) {
    counterStepInput.value =
      hunt.value.counterStep

    return
  }

  try {
    const response =
      await fetch(
        `http://localhost:5119/api/hunts/${hunt.value.id}/counter-step`,
        {
          method: 'PUT',

          headers: {
            'Content-Type':
              'application/json'
          },

          body: JSON.stringify({
            step
          })
        }
      )

    if (!response.ok) {
      throw new Error(
        'Could not update counter step'
      )
    }

    const result =
      await response.json()

    hunt.value.counterStep =
      result.counterStep

    counterStepInput.value =
      result.counterStep
  } catch (error) {
    console.error(error)

    alert(
      'Something went wrong while changing the counter step.'
    )
  }
}

onMounted(() => {
  loadHunt()
})

const phaseCount = computed(() => {
  return (
    hunt.value?.phases?.length ??
    0
  )
})

function getGameName(game) {
  return game.label || game.game
}

function getTargetDetails(target) {
  const details = []

  if (target.form) {
    details.push(
      target.form
    )
  }

  if (target.gender) {
    details.push(
      target.gender
    )
  }

  if (target.nature) {
    details.push(
      target.nature
    )
  }

  if (
    target.isAlpha === true
  ) {
    details.push(
      'Alpha'
    )
  }

  if (
    target.isAlpha === false
  ) {
    details.push(
      'Not Alpha'
    )
  }

  if (target.mark) {
    details.push(
      `${target.mark} Mark`
    )
  }

  if (
    details.length === 0
  ) {
    return 'Any variation'
  }

  return details.join(' • ')
}

function getPhasePokemon(phase) {
  if (!phase.foundShiny) {
    return 'Unknown shiny'
  }

  return phase.foundShiny.nickname
    ? `${phase.foundShiny.pokemon} “${phase.foundShiny.nickname}”`
    : phase.foundShiny.pokemon
}

function formatDate(value) {
  if (!value) {
    return '—'
  }

  return new Date(
    value
  ).toLocaleDateString()
}
</script>

<template>
  <main class="hunt-page">
    <p
      v-if="isLoading"
      class="message"
    >
      Loading hunt...
    </p>

    <p
      v-else-if="errorMessage"
      class="message error"
    >
      {{ errorMessage }}
    </p>

    <template v-else-if="hunt">
<header class="hunt-header">
  <div>
    <RouterLink
      to="/hunts"
      class="back-link"
    >
      ← Back to Hunts
    </RouterLink>

    <div class="title-row">
      <h1>
        {{ hunt.name }}
      </h1>

      <span
        class="status"
        :class="{
          completed:
            hunt.status ===
            'Completed'
        }"
      >
        {{
          hunt.status ===
          'Completed'
            ? 'Hunt Completed'
            : 'Active'
        }}
      </span>
    </div>

    <p class="started">
      Started
      {{
        formatDate(
          hunt.startedAt
        )
      }}
    </p>
  </div>

<div class="header-actions">
  <button
    type="button"
    class="edit-hunt-button"
    @click="
      router.push(
        `/hunts/${hunt.id}/edit`
      )
    "
  >
    Edit Hunt
  </button>

  <button
    type="button"
    class="delete-hunt-button"
    :disabled="isDeleting"
    @click="deleteHunt"
  >
    {{
      isDeleting
        ? 'Deleting...'
        : 'Delete Hunt'
    }}
  </button>
</div>
</header>

      <!-- CURRENT HUNT -->
      <section
        v-if="
          hunt.status ===
          'Active'
        "
        class="counter-card"
      >
        <div class="phase-heading">
          <span>
            Current Phase
          </span>

          <strong>
            Phase
            {{
              hunt.currentPhaseNumber
            }}
          </strong>
        </div>

        <!-- TARGET SPRITES -->
        <div
          v-if="
            hunt.targets?.length
          "
          class="target-sprite-row"
        >
          <article
            v-for="
              target
              in hunt.targets
            "
            :key="target.id"
            class="target-sprite-card"
          >
            <img
              v-if="
                getTargetSprite(
                  target
                )
              "
              :src="
                getTargetSprite(
                  target
                )
              "
              :alt="
                target.pokemon
              "
              class="pokemon-sprite large"
            />

            <div class="sprite-info">
              <strong>
                {{ target.pokemon }}
              </strong>

              <span
                v-if="
                  getTargetSubtitle(
                    target
                  )
                "
              >
                {{
                  getTargetSubtitle(
                    target
                  )
                }}
              </span>
            </div>
          </article>
        </div>

        <template
          v-if="
            hunt.usesCounter
          "
        >
          <div class="current-counter">
            <span>
              Current Encounters
            </span>

            <strong>
              {{
                hunt.currentEncounters
                  .toLocaleString()
              }}
            </strong>
          </div>

          <div
            class="counter-step-control"
          >
            <label
              for="counter-step"
            >
              Counter Step
            </label>

            <input
              id="counter-step"
              v-model.number="
                counterStepInput
              "
              type="number"
              min="1"
              @change="
                saveCounterStep
              "
            />
          </div>

          <div class="counter-buttons">
            <button
              type="button"
              :disabled="
                isUpdatingCounter ||
                hunt.currentEncounters ===
                  0
              "
              @click="
                adjustCounter(
                  -hunt.counterStep
                )
              "
            >
              -{{ hunt.counterStep }}
            </button>

            <button
              type="button"
              :disabled="
                isUpdatingCounter
              "
              @click="
                adjustCounter(
                  hunt.counterStep
                )
              "
            >
              +{{ hunt.counterStep }}
            </button>
          </div>
        </template>

        <div
          v-else
          class="no-counter"
        >
          This hunt does not use an
          encounter counter.
        </div>

        <button
          type="button"
          class="shiny-found-button"
          @click="
            router.push(
              `/hunts/${hunt.id}/shiny-found`
            )
          "
        >
          ✨ Shiny Found
        </button>
      </section>

      <!-- SUMMARY -->
      <section class="summary-grid">
        <article class="summary-card">
          <span>
            Current Phase
          </span>

          <strong>
            {{
              hunt.currentPhaseNumber
            }}
          </strong>
        </article>

        <article class="summary-card">
          <span>
            Total Encounters
          </span>

          <strong
            v-if="
              hunt.usesCounter
            "
          >
            {{
              hunt.totalEncounters
                .toLocaleString()
            }}
          </strong>

          <strong v-else>
            No counter
          </strong>
        </article>

        <article class="summary-card">
          <span>
            Shinies Found
          </span>

          <strong>
            {{ phaseCount }}
          </strong>
        </article>
      </section>

      <!-- PREVIOUS PHASES -->
      <section class="content-section">
        <h2>
          Previous Phases
        </h2>

        <div
          v-if="
            hunt.phases?.length
          "
          class="phase-list"
        >
          <article
            v-for="
              phase
              in hunt.phases
            "
            :key="phase.id"
            class="phase-card"
          >
            <div class="phase-left">
              <span class="phase-label">
                Phase
                {{
                  phase.phaseNumber
                }}
              </span>

              <img
                v-if="
                  getPhaseSprite(
                    phase
                  )
                "
                :src="
                  getPhaseSprite(
                    phase
                  )
                "
                :alt="
                  phase.foundShiny
                    ?.pokemon
                "
                class="pokemon-sprite medium"
              />

              <div
                class="phase-main-info"
              >
                <strong>
                  {{
                    getPhasePokemon(
                      phase
                    )
                  }}
                </strong>

                <span
                  v-if="
                    getPhaseSubtitle(
                      phase
                    )
                  "
                >
                  {{
                    getPhaseSubtitle(
                      phase
                    )
                  }}
                </span>

                <span
                  v-if="
                    phase.wasTarget
                  "
                  class="target-found"
                >
                  Target
                </span>
              </div>
            </div>

            <div class="phase-meta">
              <span
                v-if="
                  phase.encounters !==
                  null
                "
              >
                {{
                  phase.encounters
                    .toLocaleString()
                }}
                encounters
              </span>

              <span>
                {{
                  formatDate(
                    phase.completedAt
                  )
                }}
              </span>
            </div>
          </article>
        </div>

        <p
          v-else
          class="empty-text"
        >
          No shinies found during
          this hunt yet.
        </p>
      </section>

      <!-- GAMES -->
      <section class="content-section">
        <h2>
          Games & Encounter Pools
        </h2>

        <div class="games-list">
          <article
            v-for="
              game
              in hunt.games
            "
            :key="game.id"
            class="game-card"
          >
            <div class="game-header">
              <div>
                <h3>
                  {{
                    getGameName(
                      game
                    )
                  }}
                </h3>

                <p>
                  {{ game.game }}
                </p>
              </div>

              <strong
                v-if="
                  game.shinyOdds
                "
                class="odds"
              >
                1 /
                {{
                  game.shinyOdds
                }}
              </strong>
            </div>

            <div
              v-if="
                game.encounters?.length
              "
              class="encounter-list"
            >
              <div
                v-for="
                  encounter
                  in game.encounters
                "
                :key="
                  encounter.id
                "
                class="encounter-row"
              >
                <div
                  class="encounter-left"
                >
                  <img
                    v-if="
                      getEncounterSprite(
                        encounter
                      )
                    "
                    :src="
                      getEncounterSprite(
                        encounter
                      )
                    "
                    :alt="
                      encounter.pokemon
                    "
                    class="pokemon-sprite small"
                  />

                  <div
                    class="encounter-info"
                  >
                    <strong>
                      {{
                        encounter.pokemon
                      }}
                    </strong>

                    <span
                      v-if="
                        getEncounterSubtitle(
                          encounter
                        )
                      "
                    >
                      {{
                        getEncounterSubtitle(
                          encounter
                        )
                      }}
                    </span>
                  </div>
                </div>

                <strong
                  v-if="
                    encounter
                      .encounterPercentage !==
                    null
                  "
                  class="encounter-rate"
                >
                  {{
                    encounter
                      .encounterPercentage
                  }}%
                </strong>
              </div>
            </div>

            <p
              v-else
              class="empty-text"
            >
              No encounter pool
              configured.
            </p>
          </article>
        </div>
      </section>

      <!-- NOTES -->
      <section
        v-if="hunt.notes"
        class="content-section"
      >
        <h2>Notes</h2>

        <p class="notes">
          {{ hunt.notes }}
        </p>
      </section>
    </template>
  </main>
</template>

<style scoped>
.hunt-page {
  max-width: 1050px;
  margin: 0 auto;
  padding: 50px 20px;
}

.message {
  text-align: center;
  font-weight: 700;
}

.error {
  color: #b91c1c;
}

.hunt-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: 20px;

  margin-bottom: 30px;
}

.back-link {
  display: inline-block;
  margin-bottom: 15px;

  color: #6b7280;
  text-decoration: none;
  font-weight: 600;
}

.title-row {
  display: flex;
  align-items: center;
  gap: 14px;
}

.title-row h1 {
  margin: 0;
  font-size: 2.4rem;
}

.status {
  padding: 5px 10px;

  border-radius: 999px;

  background: #dcfce7;
  color: #166534;

  font-size: 0.8rem;
  font-weight: 700;
}

.status.completed {
  background: #e5e7eb;
  color: #374151;
}

.started {
  margin: 8px 0 0;
  color: #6b7280;
}

.counter-card {
  padding: 30px;

  border: 1px solid #e5e7eb;
  border-radius: 20px;

  background: white;

  box-shadow:
    0 6px 20px
    rgba(0, 0, 0, 0.06);

  text-align: center;
}

.phase-heading {
  display: flex;
  justify-content: space-between;

  color: #6b7280;
}

.phase-heading strong {
  color: #111827;
}

/* TARGET SPRITES */

.target-sprite-row {
  display: flex;
  justify-content: center;
  flex-wrap: wrap;
  gap: 16px;

  margin: 25px 0 10px;
}

.target-sprite-card {
  display: flex;
  align-items: center;
  gap: 12px;

  min-width: 180px;
  padding: 10px 15px;

  border:
    1px solid #e5e7eb;
  border-radius: 16px;

  background: #f9fafb;

  text-align: left;
}

.sprite-info {
  display: flex;
  flex-direction: column;
  gap: 2px;
}

.sprite-info span {
  color: #6b7280;
  font-size: 0.82rem;
}

/* SPRITES */

.pokemon-sprite {
  object-fit: contain;
  image-rendering: pixelated;

  flex-shrink: 0;
}

.pokemon-sprite.small {
  width: 100px;
  height: 100px;
}

.pokemon-sprite.medium {
  width: 128px;
  height: 128px;
}

.pokemon-sprite.large {
  width: 160px;
  height: 160px;
}

/* COUNTER */

.current-counter {
  margin: 25px 0 20px;
}

.current-counter span {
  display: block;

  color: #6b7280;
  font-weight: 600;
}

.current-counter strong {
  display: block;

  margin-top: 5px;

  font-size: 3.5rem;
}

.counter-step-control {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 10px;

  margin-bottom: 15px;
}

.counter-step-control label {
  color: #6b7280;

  font-size: 0.85rem;
  font-weight: 700;
}

.counter-step-control input {
  width: 80px;

  padding: 8px 10px;

  border:
    1px solid #d1d5db;
  border-radius: 8px;

  font: inherit;
  text-align: center;
}

.counter-buttons {
  display: flex;
  justify-content: center;
  gap: 12px;
}

.counter-buttons button {
  min-width: 100px;

  padding: 12px 20px;

  border: none;
  border-radius: 10px;

  background: #111827;
  color: white;

  font-weight: 700;
  cursor: pointer;
}

.counter-buttons button:disabled {
  opacity: 0.45;
  cursor: default;
}

.no-counter {
  margin: 30px 0;

  color: #6b7280;
  font-weight: 600;
}

.shiny-found-button {
  margin-top: 20px;

  padding: 13px 25px;

  border: none;
  border-radius: 12px;

  background: #111827;
  color: white;

  font-weight: 800;
  cursor: pointer;
}

/* SUMMARY */

.summary-grid {
  display: grid;

  grid-template-columns:
    repeat(3, 1fr);

  gap: 15px;

  margin-top: 22px;
}

.summary-card {
  padding: 18px;

  border:
    1px solid #e5e7eb;
  border-radius: 14px;

  background: #f9fafb;

  text-align: center;
}

.summary-card span {
  display: block;

  margin-bottom: 5px;

  color: #6b7280;
  font-size: 0.8rem;
}

.summary-card strong {
  font-size: 1.4rem;
}

/* SECTIONS */

.content-section {
  margin-top: 38px;
}

.content-section h2 {
  margin-bottom: 15px;
}

/* TARGETS */

.target-grid {
  display: grid;

  grid-template-columns:
    repeat(
      auto-fit,
      minmax(200px, 1fr)
    );

  gap: 12px;
}

.target-card {
  display: flex;
  flex-direction: column;
  gap: 5px;

  padding: 16px;

  border:
    1px solid #e5e7eb;
  border-radius: 12px;

  background: white;
}

.target-card span {
  color: #6b7280;
  font-size: 0.85rem;
}

/* PHASES */

.phase-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.phase-card {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 20px;

  padding: 15px 18px;

  border:
    1px solid #e5e7eb;
  border-radius: 14px;

  background: white;
}

.phase-left {
  display: flex;
  align-items: center;
  gap: 14px;
}

.phase-label {
  min-width: 55px;

  color: #6b7280;
  font-size: 0.8rem;
}

.phase-main-info {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  gap: 3px;
}

.phase-main-info > span:not(.target-found) {
  color: #6b7280;
  font-size: 0.8rem;
}

.target-found {
  display: inline-block;

  margin-top: 3px;
  padding: 3px 7px;

  border-radius: 999px;

  background: #dcfce7;
  color: #166534;

  font-size: 0.7rem;
  font-weight: 700;
}

.phase-meta {
  display: flex;
  gap: 15px;

  color: #6b7280;
  font-size: 0.8rem;
}

/* GAMES */

.games-list {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.game-card {
  padding: 20px;

  border:
    1px solid #e5e7eb;
  border-radius: 16px;

  background: white;
}

.game-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 15px;
}

.game-header h3 {
  margin: 0;
}

.game-header p {
  margin: 4px 0 0;
  color: #6b7280;
}

.odds {
  white-space: nowrap;
}

/* ENCOUNTERS */

.encounter-list {
  margin-top: 18px;

  border-top:
    1px solid #e5e7eb;
}

.encounter-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 15px;

  min-height: 58px;
  padding: 8px 0;

  border-bottom:
    1px solid #f3f4f6;
}

.encounter-left {
  display: flex;
  align-items: center;
  gap: 12px;
}

.encounter-info {
  display: flex;
  flex-direction: column;
  gap: 2px;
}

.encounter-info span {
  color: #6b7280;
  font-size: 0.8rem;
}

.encounter-rate {
  white-space: nowrap;
}

.empty-text {
  color: #6b7280;
}

.notes {
  padding: 16px;

  border-radius: 12px;

  background: #f9fafb;

  white-space: pre-wrap;
}

/* MOBILE */

@media (max-width: 650px) {
  .hunt-page {
    padding:
      30px 16px;
  }

  .hunt-header {
  flex-direction: column;
}

.delete-hunt-button {
  width: 100%;
}


  .title-row {
    align-items: flex-start;
    flex-direction: column;
  }

  .title-row h1 {
    font-size: 2rem;
  }

  .summary-grid {
    grid-template-columns: 1fr;
  }

  .phase-card {
    align-items: flex-start;
    flex-direction: column;
  }

  .phase-left {
    align-items: flex-start;
  }

  .phase-meta {
    flex-direction: column;
    gap: 4px;
  }

  .target-sprite-card {
    width: 100%;
    box-sizing: border-box;
  }

  .game-header {
    align-items: flex-start;
  }

  .delete-hunt-button {
  padding: 10px 16px;

  border: 1px solid #fecaca;
  border-radius: 10px;

  background: #fee2e2;
  color: #991b1b;

  font-weight: 700;
  cursor: pointer;
}

.delete-hunt-button:hover {
  background: #fecaca;
}

.delete-hunt-button:disabled {
  opacity: 0.5;
  cursor: default;
}

.header-actions {
  display: flex;
  gap: 10px;
}

.edit-hunt-button {
  padding: 10px 16px;

  border: 1px solid #d1d5db;
  border-radius: 10px;

  background: white;
  color: #374151;

  font-weight: 700;
  cursor: pointer;
}

@media (max-width: 650px) {
  .header-actions {
    width: 100%;
    flex-direction: column;
  }

  .edit-hunt-button,
  .delete-hunt-button {
    width: 100%;
  }
}
}
</style>