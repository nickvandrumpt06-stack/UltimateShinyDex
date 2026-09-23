<script setup>
defineProps({
  shiny: {
    type: Object,
    required: true
  }
})
</script>

<template>
  <div>
    <h3>{{ shiny.pokemon }}</h3>
    <p>Nickname: {{ shiny.nickname }}</p>
    <p>Nature: {{ shiny.nature }}</p>
    <p>Game: {{ shiny.game }}</p>
    <p>Ball: {{ shiny.ball }}</p>
  </div>
</template>