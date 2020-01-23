import React, { Component } from "react";
import { Layer, Line, Group, Rect } from "react-konva"
import { Text } from "./KonvaNodes";

export class Grid extends Component {
  render = () => {
    if (!this.props.enabled) {
      return (
        <Layer>
          <Rect
            x = {0}
            y = {0}
            width = {this.props.xLim}
            height = {this.props.yLim}
            fill = "#fff"
          />
        </Layer>
      )
    }

    let grid = [];
    for (let x = 0; x < this.props.xLim; x+=this.props.gridSize) {
      grid.push([x, 0, x, this.props.yLim]);
    }

    for (let y = 0; y < this.props.yLim; y+=this.props.gridSize) {
      grid.push([0, y, this.props.xLim, y]);
    }

    return (
      <Layer>
        <Rect
          x = {0}
          y = {0}
          width = {this.props.xLim}
          height = {this.props.yLim}
          fill = "#fff"
        />
        {grid.map((line, index) => {
          const text = line[0] === 0 ? line[1] : line[0];
          return (
            <Group key = {"grid-" + index}>
              <Line
                points = {line}
                stroke = "#ddd"
                strokeWidth = {this.props.lineWidth}
              />
              <Text
                x = {line[0] + 0.05}
                y = {line[1] + 0.25}
                text = {text + "m"}
                textScale = {0.025}
                fill = "#999"
              />
            </Group>
          );
        })}
      </Layer>
    );
  }
}