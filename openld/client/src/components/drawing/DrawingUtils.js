export class DrawingUtils {
    static getRelativePointerPos(stage) {
        const pointerPosition = stage.getPointerPosition();
        return {
            x: (pointerPosition.x - stage.attrs.x) / stage.attrs.scaleX,
            y: stage.attrs.offsetY - (pointerPosition.y - stage.attrs.y) / Math.abs(stage.attrs.scaleY)
        };
    }

    static getNearestSnapPos(pos, snapGridSize) {
       return {
        x: Math.round(pos.x / snapGridSize) * snapGridSize,
        y: Math.round(pos.y / snapGridSize) * snapGridSize
      };
    }
}