import { createRouter, createWebHistory } from 'vue-router'

import HomeView from '../views/HomeView.vue'
import AddShinyView from '../views/AddShinyView.vue'
import EditShinyView from '../views/EditShinyView.vue'
import BackupView from '../views/BackupView.vue'
import StatsView from '../views/StatsView.vue'
import MostOwnedView from '../views/MostOwnedView.vue'
import GameStatsView from '../views/GameStatsView.vue'
import NatureStatsView from '../views/NatureStatsView.vue'
import BallStatsView from '../views/BallStatsView.vue'
import DexSettingsView from '../views/DexSettingsView.vue'
import HuntsView from '../views/HuntsView.vue'
import HuntDetailView from '../views/HuntDetailView.vue'

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
},
{
  path: '/stats/natures',
  name: 'nature-stats',
  component: NatureStatsView
},
{
  path: '/stats/balls',
  name: 'ball-stats',
  component: BallStatsView
},
{
  path: '/dex-settings',
  name: 'dex-settings',
  component: DexSettingsView
},
{
  path: '/hunts',
  name: 'hunts',
  component: HuntsView
},
{
  path: '/hunts/new',
  name: 'create-hunt',
  component: () => import('../views/CreateHuntView.vue')
},
{
  path: '/hunts/:id/shiny-found',
  name: 'shiny-found',
  component: () =>
    import('../views/ShinyFoundView.vue')
},
{
  path: '/hunts/:id',
  name: 'hunt-detail',
  component: HuntDetailView
},
  ]
})

export default router