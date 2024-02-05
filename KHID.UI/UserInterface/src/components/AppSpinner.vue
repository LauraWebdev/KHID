<template>
    <div class="app-spinner">
        <span class="icon ri-loader-5-line rotating"></span>
        <span class="icon ri-circle-line static"></span>
    </div>
</template>

<script setup></script>

<style lang="scss" scoped>
.app-spinner {
    width: 32px;
    height: 32px;
    position: relative;

    & .icon {
        position: absolute;
        width: 48px;
        height: 48px;
        display: flex;
        top: -8px;
        left: -8px;

        &.rotating {
            align-items: flex-start;
            justify-content: flex-end;
            font-size: 32px;
            animation: rotation 1s linear infinite;
        }
        &.static {
            align-items: center;
            justify-content: center;
            font-size: 34px;
            opacity: 0.1;
        }
    }
}

@keyframes rotation {
    0% {
        transform: rotate(0deg);
    }
    100% {
        transform: rotate(360deg);
    }
}
</style>
