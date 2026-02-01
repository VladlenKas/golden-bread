<script setup lang="ts">
import TheHeader from './components/layout/TheHeader.vue'
import TheMain from './components/layout/TheMain.vue'
import TheFooter from './components/layout/TheFooter.vue'
import { Toaster } from '@/components/ui/toast'
</script>


<template>
  <TheHeader />
  <TheMain />
  <TheFooter />
  <Toaster />
</template>
