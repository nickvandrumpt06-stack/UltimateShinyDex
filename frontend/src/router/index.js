import { createRouter, createWebHistory } from 'vue-router'

import HomeView from '../views/HomeView.vue'
import AddShinyView from '../views/AddShinyView.vue'
import EditShinyView from '../views/EditShinyView.vue'
import BackupView from '../views/BackupView.vue'
import StatsView from '../views/StatsView.vue'
import MostOwnedView from '../views/MostOwnedView.vue'
import GameStatsView from '../views/GameStatsView.vue'

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
    },
    {
      path: '/backup',
      name: 'backup',
      component: BackupView
    },
    {
      path: '/stats',
      name: 'stats',
      component: StatsView
    },
    {
  path: '/stats/most-owned',
  name: 'most-owned',
  component: MostOwnedView
},
{
  path: '/stats/games',
  name: 'game-stats',
  component: GameStatsView
}
  ]
})

export default router