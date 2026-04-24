<script setup lang="ts">
import { Toaster } from '@/shared/ui/toast'
import { default as Layout } from './AppLayout.vue'
import { default as Header } from './AppHeader.vue';
import { default as Footer } from './AppFooter.vue';
</script>

<template>
  <Header/>
  <Layout />
  <Toaster />
  <Footer/>
</template>
