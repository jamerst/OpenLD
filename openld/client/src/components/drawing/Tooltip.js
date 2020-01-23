import React, { Component } from "react";
import { Label, Tag, Text } from "react-konva";


export class Tooltip extends Component {
  render = () => {
    return (
      <Label
        position = {this.props.position}
        scale = {{x: this.props.scale, y: -this.props.scale}}
        visible = {this.props.visible}
      >
        <Tag fill="#ddd"/>
        <Text
          key = "tooltip"
          text = {this.props.text}
          fill = "#000"
          padding = {2}
        />
      </Label>
    )
  }
}