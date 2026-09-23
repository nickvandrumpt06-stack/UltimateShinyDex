<script setup>
import { RouterLink, RouterView } from 'vue-router'
</script>

<template>
  <nav>
    <RouterLink to="/">Home</RouterLink>
    |
    <RouterLink to="/add">Add Shiny</RouterLink>
  </nav>

  <RouterView />
</template>