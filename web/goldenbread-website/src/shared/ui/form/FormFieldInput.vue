<script setup lang="ts">
import { FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/shared/ui/form';
import { Input } from '@/shared/ui/input';

defineProps<{
  name: string;
  label: string;
  placeholder?: string;
  type?: string;
}>();
</script>

<template>
  <FormField v-slot="{ componentField }" :name="name">
    <FormItem v-auto-animate>
      <FormLabel>{{ label }}</FormLabel>
      <FormControl>
        <Input :type="type" :placeholder="placeholder" v-bind="componentField" />
      </FormControl>
      <FormMessage />
    </FormItem>
  </FormField>
</template>