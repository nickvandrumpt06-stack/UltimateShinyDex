<script setup>
import { RouterLink, RouterView } from 'vue-router'
</script>

<template>
  <header class="navbar">
    <div class="navbar-inner">
      <h1 class="logo">Ultimate Shiny Dex</h1>

      <nav>
        <RouterLink to="/">My Shinies</RouterLink>
        <RouterLink to="/add">Add Shiny</RouterLink>
      </nav>
    </div>
  </header>

  <RouterView />
</template>

<style scoped>
.navbar {
  width: 100%;
  border-bottom: 1px solid #ddd;
}

.navbar-inner {
  max-width: 1200px;
  margin: 0 auto;
  padding: 20px;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.logo {
  margin: 0;
}

nav {
  display: flex;
  gap: 20px;
}

nav a {
  text-decoration: none;
}
</style>