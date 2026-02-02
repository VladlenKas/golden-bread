<script setup lang="ts">
import { Toaster } from '@/components/ui/toast'
import TheHeader from './components/layout/TheHeader.vue'
import TheFooter from './components/layout/TheFooter.vue'
import TheMain from './components/layout/TheMain.vue'
</script>


<template>
  <TheHeader />
  <TheMain />
  <TheFooter />
  <Toaster />
</template>
