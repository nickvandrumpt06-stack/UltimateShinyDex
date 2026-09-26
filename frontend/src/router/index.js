import { createRouter, createWebHistory } from 'vue-router'
import EditShinyView from '../views/EditShinyView.vue'

import HomeView from '../views/HomeView.vue'
import AddShinyView from '../views/AddShinyView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),

  routes: [
    {
      path: '/',
      name: 'home',
      component: HomeView
    },
    {
      path: '/add',
      name: 'add-shiny',
      component: AddShinyView
    },
    {
      path: '/edit/:id',
      name: 'edit-shiny',
      component: EditShinyView
    }
  ]
})

export default router