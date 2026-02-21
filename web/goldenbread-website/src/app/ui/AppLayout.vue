<script setup lang="ts">
import { RouterView } from 'vue-router';
</script>

<template>
  <div class="flex flex-col min-h-screen">
    <main class="flex-1 bg-muted/40">
      <RouterView />
    </main>
  </div>
</template>