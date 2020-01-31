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
    const majorHeight = Math.log10(this.props.viewDimension) / 3
    const minorHeight = majorHeight / 1.5;
    const subMinorHeight = majorHeight / 3
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
                points = {[this.props.x+mark, this.props.y, this.props.x+mark, this.props.y+majorHeight]}
                stroke = "#f00"
                strokeWidth = {0.025}
              />
              <Text
                x = {this.props.x+mark+0.05}
                y = {this.props.y+majorHeight}
                textScale = {Math.log10(this.props.viewDimension)/85}
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
                points = {[this.props.x+mark, this.props.y, this.props.x+mark, this.props.y+minorHeight]}
                stroke = "#f00"
                strokeWidth = {0.025}
              />
              <Text
                x = {this.props.x+mark+0.05}
                y = {this.props.y+minorHeight}
                textScale = {Math.log10(this.props.viewDimension)/100}
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
              points = {[this.props.x+mark, this.props.y, this.props.x+mark, this.props.y+subMinorHeight]}
              stroke = "#f00"
              strokeWidth = {0.025}
            />
          )
        })}
      </Group>
    )
  }
}