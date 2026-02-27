<script setup>
import {
  TooltipTrigger,
  Tooltip,
  TooltipProvider,
  TooltipContent,
} from '.'
</script>

<template>
  <TooltipProvider>
    <Tooltip>
      <TooltipTrigger as-child>
        <span class="flex items-center gap-1.5 px-2 py-1 -mx-1 rounded-md cursor-help hover:bg-muted transition-colors">
          <slot name="icon" />
          <slot name="trigger" />
        </span>
      </TooltipTrigger>
      <TooltipContent>
        <slot name="content" />
      </TooltipContent>
    </Tooltip>
  </TooltipProvider>
</template>