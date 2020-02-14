import React, { Component } from "react";
import { Label, Tag, Text } from "react-konva";
import { DrawingUtils } from "./DrawingUtils";

export class DrawingLabel extends Component {
  constructor(props) {
    super(props);

    this.state = {
      startPos: {x: 0, y: 0}
    };
  }

  render = () => {
    const colour = typeof this.props.data.colour === "undefined" ? "#fff" : this.props.data.colour;

    return (
      <Label
        scale = {{x: Math.log10(this.props.viewDimension)/65, y: -Math.log10(this.props.viewDimension)/65}}
        x={this.props.data.position.x}
        y={this.props.data.position.y}
        onClick = {this.onClick}
        onMouseEnter = {() => this.props.setCursor("pointer")}
        onMouseLeave = {() => this.props.setCursor("grab")}
        draggable

        onDragStart = {this.handleDragStart}
        onDragMove = {this.handleDragMove}
        onDragEnd = {this.handleDragEnd}
      >
        <Tag fill="#fff"
          strokeEnabled = {colour !== "#fff"}
          stroke={colour}
          strokeWidth={2}
        />
        <Text
          key={`flc-${this.props.data.id}`}
          align = "left"
          padding = {2}
          text = {this.props.data.text}
          fill = "#000"
        />
      </Label>
    );
  }

  onClick = (event) => {
    if (this.props.hubConnected) {
      event.cancelBubble = true;
      this.props.deselectObject(this.props.selectedFixtureStructure);
      this.props.onSelectObject("label", this.props.data.id);

      if (this.props.selectedTool === "none") {
        this.props.setLabelColour(this.props.data.id, "#007bff");
        this.props.setHintText("Modify text above.\nPress delete to remove label.\nPress Ctrl+C to copy label.")
      }
    }
  }

  handleDragStart = (event) => {
    event.cancelBubble = true;
    this.setState({startPos: this.props.data.position});
    this.props.setHintText("Release to confirm position");
  }

  handleDragMove = (event) => {
    event.cancelBubble = true;

    const pos = event.target.position();
    const snapPos = DrawingUtils.getNearestSnapPos(pos, this.props.snapGridSize);

    this.props.setTooltip({x: pos.x, y: pos.y + 20 / this.props.scale}, true, `(${snapPos.x.toFixed(1)},${snapPos.y.toFixed(1)})`);
  }

  handleDragEnd = (event) => {
    event.cancelBubble = true;

    const snapPos = DrawingUtils.getNearestSnapPos(event.target.position(), this.props.snapGridSize);

    this.props.onMoveLabel(this.props.data.id, snapPos, this.state.startPos);
    console.log(this.state.startPos);
    this.props.setTooltip({x: 0, y: 0}, false, "");
  }
}