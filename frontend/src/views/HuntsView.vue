<script setup>
import { ref, computed, onMounted } from 'vue'
import { RouterLink } from 'vue-router'

const hunts = ref([])
const isLoading = ref(true)
const errorMessage = ref('')

async function loadHunts() {
  try {
    const response = await fetch(
      'http://localhost:5119/api/hunts'
    )

    if (!response.ok) {
      throw new Error('Could not load hunts')
    }

    hunts.value = await response.json()
  } catch (error) {
    console.error(error)

    errorMessage.value =
      'Could not load your hunts.'
  } finally {
    isLoading.value = false
  }
}

onMounted(() => {
  loadHunts()
})

const activeHunts = computed(() => {
  return hunts.value.filter(
    (hunt) => hunt.status === 'Active'
  )
})

const completedHunts = computed(() => {
  return hunts.value.filter(
    (hunt) => hunt.status === 'Completed'
  )
})

function getGameNames(hunt) {
  if (!hunt.games?.length) {
    return 'No games selected'
  }

  return hunt.games
    .map((game) => game.label || game.game)
    .join(' • ')
}

function getTargetNames(hunt) {
  if (!hunt.targets?.length) {
    return 'No targets'
  }

  return hunt.targets
    .map((target) => {
      const details = []

      if (target.form) {
        details.push(target.form)
      }

      if (target.gender) {
        details.push(target.gender)
      }

      if (details.length === 0) {
        return target.pokemon
      }

      return `${target.pokemon} (${details.join(', ')})`
    })
    .join(' • ')
}
</script>

<template>
  <main class="hunts-page">
    <header class="page-header">
      <div>
        <h1>Shiny Hunts</h1>

        <p>
          Track your current hunts, phases and targets.
        </p>
      </div>

      <RouterLink
        to="/hunts/new"
        class="create-button"
      >
        + Create Hunt
      </RouterLink>
    </header>

    <p
      v-if="isLoading"
      class="message"
    >
      Loading hunts...
    </p>

    <p
      v-else-if="errorMessage"
      class="message error"
    >
      {{ errorMessage }}
    </p>

    <template v-else>
      <section
        v-if="activeHunts.length > 0"
        class="hunt-section"
      >
        <div class="section-header">
          <h2>Active Hunts</h2>

          <span>
            {{ activeHunts.length }}
          </span>
        </div>

        <div class="hunt-grid">
          <RouterLink
            v-for="hunt in activeHunts"
            :key="hunt.id"
            :to="`/hunts/${hunt.id}`"
            class="hunt-card"
          >
            <div class="card-top">
              <span class="status active">
                Active
              </span>

              <span class="phase">
                Phase {{ hunt.currentPhaseNumber }}
              </span>
            </div>

            <h3>
              {{ hunt.name }}
            </h3>

            <p class="games">
              {{ getGameNames(hunt) }}
            </p>

            <div class="targets">
              <span class="label">
                Targets
              </span>

              <p>
                {{ getTargetNames(hunt) }}
              </p>
            </div>

            <div class="hunt-stats">
              <div>
                <span class="stat-label">
                  Current
                </span>

                <strong v-if="hunt.usesCounter">
                  {{ hunt.currentEncounters.toLocaleString() }}
                </strong>

                <strong v-else>
                  No counter
                </strong>
              </div>

              <div>
                <span class="stat-label">
                  Total
                </span>

                <strong v-if="hunt.usesCounter">
                  {{ hunt.totalEncounters.toLocaleString() }}
                </strong>

                <strong v-else>
                  —
                </strong>
              </div>

              <div>
                <span class="stat-label">
                  Finds
                </span>

                <strong>
                  {{ hunt.phases?.length ?? 0 }}
                </strong>
              </div>
            </div>
          </RouterLink>
        </div>
      </section>

      <section
        v-if="completedHunts.length > 0"
        class="hunt-section"
      >
        <div class="section-header">
          <h2>Completed Hunts</h2>

          <span>
            {{ completedHunts.length }}
          </span>
        </div>

        <div class="hunt-grid">
          <RouterLink
            v-for="hunt in completedHunts"
            :key="hunt.id"
            :to="`/hunts/${hunt.id}`"
            class="hunt-card completed"
          >
            <div class="card-top">
              <span class="status completed-status">
                Hunt Completed
              </span>

              <span class="phase">
                {{ hunt.phases?.length ?? 0 }}
                phases
              </span>
            </div>

            <h3>
              {{ hunt.name }}
            </h3>

            <p class="games">
              {{ getGameNames(hunt) }}
            </p>

            <div class="targets">
              <span class="label">
                Targets
              </span>

              <p>
                {{ getTargetNames(hunt) }}
              </p>
            </div>
          </RouterLink>
        </div>
      </section>

      <section
        v-if="
          activeHunts.length === 0 &&
          completedHunts.length === 0
        "
        class="empty-state"
      >
        <h2>No hunts yet</h2>

        <p>
          Create your first shiny hunt and start tracking
          your phases.
        </p>

        <RouterLink
          to="/hunts/new"
          class="create-button"
        >
          Create First Hunt
        </RouterLink>
      </section>
    </template>
  </main>
</template>

<style scoped>
.hunts-page {
  max-width: 1100px;
  margin: 0 auto;
  padding: 50px 20px;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 20px;

  margin-bottom: 40px;
}

.page-header h1 {
  margin: 0;
  font-size: 2.4rem;
}

.page-header p {
  margin: 8px 0 0;
  color: #6b7280;
}

.create-button {
  display: inline-block;

  padding: 11px 18px;

  border-radius: 10px;

  background: #111827;
  color: white;

  text-decoration: none;
  font-weight: 700;
}

.message {
  text-align: center;
  font-weight: 600;
}

.error {
  color: #b91c1c;
}

.hunt-section {
  margin-top: 38px;
}

.section-header {
  display: flex;
  align-items: center;
  gap: 10px;

  margin-bottom: 16px;
}

.section-header h2 {
  margin: 0;
}

.section-header span {
  padding: 4px 9px;

  border-radius: 999px;

  background: #e5e7eb;

  font-size: 0.8rem;
  font-weight: 700;
}

.hunt-grid {
  display: grid;
  grid-template-columns:
    repeat(auto-fit, minmax(280px, 1fr));
  gap: 18px;
}

.hunt-card {
  padding: 22px;

  border: 1px solid #e5e7eb;
  border-radius: 18px;

  background: white;

  color: inherit;
  text-decoration: none;

  box-shadow: 0 5px 16px rgba(0, 0, 0, 0.06);

  transition:
    transform 0.2s ease,
    box-shadow 0.2s ease;
}

.hunt-card:hover {
  transform: translateY(-3px);

  box-shadow:
    0 10px 25px rgba(0, 0, 0, 0.1);
}

.hunt-card.completed {
  opacity: 0.85;
}

.card-top {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 10px;

  margin-bottom: 14px;
}

.status {
  padding: 5px 10px;

  border-radius: 999px;

  font-size: 0.75rem;
  font-weight: 700;
}

.status.active {
  background: #dcfce7;
  color: #166534;
}

.completed-status {
  background: #e5e7eb;
  color: #374151;
}

.phase {
  color: #6b7280;
  font-size: 0.8rem;
  font-weight: 600;
}

.hunt-card h3 {
  margin: 0 0 8px;
  font-size: 1.35rem;
}

.games {
  margin: 0;

  color: #6b7280;
  font-size: 0.9rem;
}

.targets {
  margin-top: 20px;
}

.targets .label {
  display: block;

  margin-bottom: 5px;

  color: #6b7280;
  font-size: 0.75rem;
  font-weight: 700;

  text-transform: uppercase;
}

.targets p {
  margin: 0;
}

.hunt-stats {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 8px;

  margin-top: 22px;
}

.hunt-stats div {
  padding: 10px 8px;

  border-radius: 10px;

  background: #f9fafb;

  text-align: center;
}

.stat-label {
  display: block;

  margin-bottom: 4px;

  color: #6b7280;
  font-size: 0.7rem;
}

.empty-state {
  padding: 70px 20px;

  border: 1px dashed #d1d5db;
  border-radius: 18px;

  text-align: center;
}

.empty-state h2 {
  margin-top: 0;
}

.empty-state p {
  margin-bottom: 24px;
  color: #6b7280;
}

@media (max-width: 650px) {
  .hunts-page {
    padding: 30px 16px;
  }

  .page-header {
    align-items: flex-start;
    flex-direction: column;
  }

  .page-header h1 {
    font-size: 2rem;
  }

  .hunt-grid {
    grid-template-columns: 1fr;
  }
}
</style>