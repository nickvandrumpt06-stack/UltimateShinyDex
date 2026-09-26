import { RouterLink } from 'vue-router'

<script setup>
defineProps({
  shiny: {
    type: Object,
    required: true
  }
})

const emit = defineEmits(['deleted'])

async function deleteshiny(id) {
  await fetch(`http://localhost:5119/api/shinies/${id}`, {
    method: 'DELETE'
  })

  emit('deleted')
}
</script>

<template>
  <div>
    <h3>{{ shiny.pokemon }}</h3>
    <p>Nickname: {{ shiny.nickname }}</p>
    <p>Nature: {{ shiny.nature }}</p>
    <p>Game: {{ shiny.game }}</p>
    <p>Ball: {{ shiny.ball }}</p>
    <p>Method: {{ shiny.method }}</p>
    <p>Encounters: {{ shiny.encounters }}</p>
    <p>Is Alpha: {{ shiny.isAlpha }}</p>
    <p>Mark: {{ shiny.mark }}</p>

    <button @click="deleteshiny(shiny.id)">
      Delete
    </button>

    <RouterLink :to="`/edit/${shiny.id}`">
  Edit
</RouterLink>
  </div>
</template>