<script setup>
import ShinyCard from '../components/ShinyCard.vue'
import { shinies } from '../data/shinies'
</script>

<template>
  <main>
    <h1>Ultimate Shiny Dex</h1>
    <h2>My Shinies</h2>

    <ShinyCard
      v-for="shiny in shinies"
      :key="shiny.id"
      :shiny="shiny"
    />
  </main>
</template>