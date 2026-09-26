<script setup>
import { RouterLink } from 'vue-router'

defineProps({
  shiny: {
    type: Object,
    required: true
  }
})

const emit = defineEmits(['deleted'])

async function deleteShiny(id) {
  await fetch(`http://localhost:5119/api/shinies/${id}`, {
    method: 'DELETE'
  })

  emit('deleted')
}
</script>

<template>
  <article class="shiny-card">
    <div class="sprite-placeholder">
      <span>{{ shiny.pokemon }}</span>
    </div>

    <div class="card-header">
      <h2>{{ shiny.pokemon }}</h2>

      <p v-if="shiny.nickname" class="nickname">
        "{{ shiny.nickname }}"
      </p>
    </div>

    <div class="badges">
      <span v-if="shiny.isAlpha" class="badge">
        Alpha
      </span>

      <span v-if="shiny.mark" class="badge">
        {{ shiny.mark }}
      </span>
    </div>

    <div class="details">
      <p>
        <strong>Nature</strong>
        <span>{{ shiny.nature }}</span>
      </p>

      <p>
        <strong>Game</strong>
        <span>{{ shiny.game }}</span>
      </p>

      <p>
        <strong>Ball</strong>
        <span>{{ shiny.ball }}</span>
      </p>

      <p>
        <strong>Method</strong>
        <span>{{ shiny.method }}</span>
      </p>

      <p v-if="shiny.encounters !== null">
        <strong>Encounters</strong>
        <span>{{ shiny.encounters }}</span>
      </p>
    </div>

    <div class="actions">
      <RouterLink
        class="edit-button"
        :to="`/edit/${shiny.id}`"
      >
        Edit
      </RouterLink>

      <button
        class="delete-button"
        @click="deleteShiny(shiny.id)"
      >
        Delete
      </button>
    </div>
  </article>
</template>

<style scoped>
.shiny-card {
  display: flex;
  flex-direction: column;
  padding: 20px;
  border-radius: 18px;
  background: #ffffff;
  border: 1px solid #e5e7eb;
  box-shadow: 0 6px 18px rgba(0, 0, 0, 0.08);
  transition:
    transform 0.2s ease,
    box-shadow 0.2s ease;
}

.shiny-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 10px 24px rgba(0, 0, 0, 0.12);
}

.sprite-placeholder {
  height: 160px;
  border-radius: 14px;
  background: #f3f4f6;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 18px;
  text-align: center;
  color: #6b7280;
}

.card-header h2 {
  margin: 0;
  font-size: 1.5rem;
}

.nickname {
  margin: 4px 0 0;
  color: #6b7280;
  font-style: italic;
}

.badges {
  display: flex;
  gap: 8px;
  flex-wrap: wrap;
  margin: 14px 0;
}

.badge {
  padding: 5px 10px;
  border-radius: 999px;
  background: #eef2ff;
  font-size: 0.8rem;
  font-weight: 600;
}

.details {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.details p {
  display: flex;
  justify-content: space-between;
  gap: 12px;
  margin: 0;
}

.details strong {
  color: #4b5563;
}

.details span {
  text-align: right;
}

.actions {
  display: flex;
  gap: 10px;
  margin-top: 20px;
}

.edit-button,
.delete-button {
  flex: 1;
  padding: 10px 14px;
  border-radius: 10px;
  font-size: 0.95rem;
  text-align: center;
  cursor: pointer;
}

.edit-button {
  text-decoration: none;
  background: #111827;
  color: white;
}

.delete-button {
  border: none;
  background: #fee2e2;
  color: #991b1b;
}
</style>