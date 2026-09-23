<script setup>
import { ref, computed } from 'vue'
import ShinyCard from '../components/ShinyCard.vue'
import { shinies } from '../data/shinies'

const search = ref('')

const filteredShinies = computed(() => {
  return shinies.value.filter((shiny) =>
    shiny.pokemon.toLowerCase().includes(search.value.toLowerCase())
  )
})
</script>

<template>
  <main>
    <h1>Ultimate Shiny Dex</h1>
    <h2>My Shinies</h2>

    <input
      type="text"
      v-model="search"
      placeholder="Search Pokémon..."
    />

    <ShinyCard
      v-for="shiny in filteredShinies"
      :key="shiny.id"
      :shiny="shiny"
    />
  </main>
</template>