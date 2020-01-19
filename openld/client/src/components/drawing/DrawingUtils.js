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

    static lineAngle(point1, point2) {
        if (typeof point1 === "undefined" || typeof point2 === "undefined") {
            return 0;
        }

        const dx = point1.x - point2.x;
        const dy = point1.y - point2.y;
        return Math.atan2(dy, dx) * 180 / Math.PI;
    }

    static nearestLinePoint(linePoints, point) {
        let nearestDistance = Infinity
        let nearestPoint = linePoints[0];

        // find the nearest point by checking each segment individually
        for (let i = 0; i < linePoints.length - 1; i++) {
            const intersection = this.intersection(linePoints[i], linePoints[i+1], point);
            const distance = this.vectorLen(intersection, point);

            if (distance < nearestDistance) {
                nearestPoint = intersection;
                nearestDistance = distance;
            }
        }

        return nearestPoint;
    }

    static intersection(A, B, P) {
        if (typeof B === "undefined") {
            return A;
        }

        // distance A->B
        const AB = {
            x: B.x - A.x,
            y: B.y - A.y
        };

        // fraction along A->B that intersection occurs at
        const k = ((P.x - A.x) * AB.x + (P.y - A.y) * AB.y) / (AB.x * AB.x + AB.y * AB.y);

        // if intersection is outside AB, return nearest end point
        if (k < 0) {
            return A;
        } else if (k > 1) {
            return B;
        }

        // return starting point + fraction * total length
        return {
            x: A.x + k * AB.x,
            y: A.y + k * AB.y
          };
    }

    static vectorLen(A, B) {
        return Math.sqrt(Math.pow(A.x - B.x, 2) + Math.pow(A.y - B.y, 2));
    }

    static moveFixtures(fixtures, change) {
        fixtures.forEach((f, index, fixtures) => {
            fixtures[index].position = {x: f.position.x + change.x, y: f.position.y + change.y}
        })

        return fixtures;
    }
}