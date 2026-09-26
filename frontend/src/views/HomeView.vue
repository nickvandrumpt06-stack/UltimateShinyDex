<script setup>
import { ref, computed, onMounted } from 'vue'
import ShinyCard from '../components/ShinyCard.vue'

const shinies = ref([])

async function loadShinies() {
  const response = await fetch('http://localhost:5119/api/shinies')
  const data = await response.json()

  shinies.value = data
}

onMounted(() => {
  loadShinies()
})

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
  @deleted="loadShinies"
/>
  </main>
</template>