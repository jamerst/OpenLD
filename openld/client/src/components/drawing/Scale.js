import React, { Component } from "react";
import { Line, Group } from "react-konva";
import { Text } from "./KonvaNodes";

export class Scale extends Component {
  static defaultProps = {
    majorMarks: [0, 5, 10],
    minorMarks: [1, 2, 3, 4, 6, 7, 8, 9],
    subMinorMarks: [.5, 1.5, 2.5, 3.5, 4.5, 5.5, 6.5, 7.5, 8.5, 9.5]
  }

  render = () => {
    return (
      <Group>
        <Line
          points = {[this.props.x, this.props.y, this.props.x+10, this.props.y]}
          stroke = "#f00"
          strokeWidth = {0.05}
        />
        {this.props.majorMarks.map(mark => {
          return (
            <Group key={`Mm-${mark}`}>
              <Line
                points = {[this.props.x+mark, this.props.y, this.props.x+mark, this.props.y+.75]}
                stroke = "#f00"
                strokeWidth = {0.025}
              />
              <Text
                x = {this.props.x+mark}
                y = {this.props.y+1}
                textScale = {0.025}
                fill = "#f00"
                text = {`${mark}m`}
              />
            </Group>
          )
        })}
        {this.props.minorMarks.map(mark => {
          return (
            <Group key={`mm-${mark}`}>
              <Line
                points = {[this.props.x+mark, this.props.y, this.props.x+mark, this.props.y+.5]}
                stroke = "#f00"
                strokeWidth = {0.025}
              />
              <Text
                x = {this.props.x+mark}
                y = {this.props.y+0.75}
                textScale = {0.025}
                fill = "#f00"
                text = {`${mark}m`}
              />
            </Group>
          )
        })}
        {this.props.subMinorMarks.map(mark => {
          return (
            <Line
              key={`smm-${mark}`}
              points = {[this.props.x+mark, this.props.y, this.props.x+mark, this.props.y+.25]}
              stroke = "#f00"
              strokeWidth = {0.025}
            />
          )
        })}
      </Group>
    )
  }
}