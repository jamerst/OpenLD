import React, { Component } from "react";
import { Circle, Group, Image, Label, Tag, Text } from "react-konva";
// import { Text } from "./KonvaNodes";

export class RiggedFixture extends Component {
  constructor(props) {
    super(props);

    this.state = {
      symbol: null,
      symbolWidth: 0,
      symbolHeight: 0
    }
  }

  componentDidMount() {
    const image = new window.Image();
    image.src = "/api/fixture/GetSymbol/" + this.props.fixtureId;
    image.onload = () => {
      let width, height;
      if (image.width >= image.height) {
        width = 1.25;
        height = image.height * width / image.width
      } else {
        height = 1.25;
        width = image.width * height / image.height
      }

      this.setState({
        symbol: image,
        symbolWidth: width,
        symbolHeight: height
      });
      this.props.onLoad(this.props.data.id);
    }
  }

  render = () => {
    const colour = typeof this.props.data.highlightColour === "undefined" ? "#000" : this.props.data.highlightColour;

    let highlight;
    if (colour !== "#000") {
      highlight = (
        <Circle
          width = {1.75}
          height = {1.75}
          fill = {colour}
          opacity = {.5}
        />
      );
    }

    let frontLabel, backLabel;
    if (this.props.data.colour) {
      frontLabel = (
        <Label
          scale = {{x: 0.04, y: -0.04}}
          x={-1}
          y={this.state.symbolHeight + 0.25}

        >
          <Tag fill="#fff"/>
          <Text
            key={"flc-" + this.props.data.id}
            width = {50}
            align = "center"
            padding = {1}
            text = {this.props.data.colour}
            fill = "#000"
          />
        </Label>
      )
    }

    if (this.props.data.name || this.props.data.universe || this.props.data.address) {
      let text = "";
      if (this.props.data.name && (this.props.data.universe || this.props.data.address)) {
        text = `${this.props.data.name}\n${this.props.data.universe}-${this.props.data.address}`
      } else if (this.props.data.name) {
        text = `${this.props.data.name}`;
      } else {
        text = `${this.props.data.universe}-${this.props.data.address}`;
      }

      backLabel = (
        <Label
          scale = {{x: 0.04, y: -0.04}}
          x={-1}
          y={-(this.state.symbolHeight/2 + 0.25)}

        >
          <Tag fill="#fff"/>
          <Text
            key={"fl-" + this.props.data.id}
            width = {50}
            align = "center"
            padding = {1}
            text = {text}
            fill = "#000"
          />
        </Label>
      )
    }

    return (
      <Group
        x = {this.props.data.position.x}
        y = {this.props.data.position.y}
        rotation = {360-this.props.data.angle}
      >
        {highlight}
        <Image
          offset = {{x: this.state.symbolWidth/2, y: this.state.symbolHeight/2}}
          scaleY = {-1}
          width = {this.state.symbolWidth}
          height = {this.state.symbolHeight}
          image = {this.state.symbol}
          onMouseEnter = {this.handleMouseEnter}
          onMouseLeave = {this.handleMouseLeave}
          onClick = {this.handleClick}

        />
        {frontLabel}
        {backLabel}
      </Group>
    );
  }

  handleMouseEnter = (event) => {
    event.target.scale({x: 1.1, y: -1.1});
    event.target.getLayer().draw();

    if (this.props.selectedTool === "none" && !this.props.selected) {
      this.props.setCursor("pointer");
      this.props.setHintText("Click to select fixture");
    }
  }

  handleMouseLeave = (event) => {
    event.target.scale({x: 1, y: -1});
    event.target.getLayer().draw();

    if (this.props.selectedTool === "none") {
      this.props.setCursor("grab");

      if (!this.props.selected) {
        this.props.setHintText("");
      }
    }
  }

  handleClick = (event) => {
    if (this.props.hubConnected) {
      event.cancelBubble = true;
      this.props.deselectObject(this.props.selectedFixtureStructure);
      this.props.setSelectedFixtureStructure(this.props.structureId);
      this.props.onSelectObject("fixture", this.props.structureId, this.props.data.id);

      if (this.props.selectedTool === "none") {
        this.props.setFixtureColour(this.props.data.id, "#007bff");
        this.props.setHintText("Modify fixture properties above.\nPress delete to remove fixture.")
      }
    }
  }
}