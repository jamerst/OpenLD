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

    static getDifference(pos1, pos2) {
        return {
            x: pos1.x - pos2.x,
            y: pos1.y - pos2.y
        }
    }

    static movePoints(points, change) {
        points.forEach(p => {
            p.x += change.x;
            p.y += change.y
        });

        return points;
    }

    static arrayPointsToObject(points) {
        let newPoints = [];

        for (let i = 0; i < points.length; i += 2) {
            newPoints.push({x: points[i], y: points[i + 1]});
        }

        return newPoints;
    }
}