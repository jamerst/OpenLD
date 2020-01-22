import React, { Component } from "react";
import { Group, Layer, Line, Rect, Label, Tag, Text as KonvaText, Circle, Image } from "react-konva";
import { DrawingUtils } from "./DrawingUtils";
import { Text } from "./KonvaNodes";

export class View extends Component {
  render = () => {
    if (typeof(this.props.data) === "undefined" || !this.props.data.structures) {
      return null;
    }

    return (<Layer>
      {this.props.data.structures.map(structure => {
        return (
          <Structure
            key={"s-" + structure.id}
            id={structure.id}
            points={structure.geometry.points}
            fixtures={structure.fixtures}
            name={structure.name}
            snapGridSize = {this.props.snapGridSize}
            setTooltip = {this.props.setTooltip}
            setHintText = {this.props.setHintText}
            updatePoints = {this.props.updatePoints}
            onDragStart = {this.props.onDragStart}
            onDragMove = {this.props.onDragMove}
            onDragEnd = {this.props.onDragEnd}
            setCursor = {this.props.setCursor}
            scale = {this.props.scale}
            onSelectObject = {this.props.onSelectObject}
            onStructureDelete = {this.props.onStructureDelete}
            deselectObject = {this.props.deselectObject}
            selected = {this.props.selectedObjectId === structure.id && this.props.selectedObjectType === "structure"}
            selectedObjectType = {this.props.selectedObjectType}
            selectedObjectId = {this.props.selectedObjectId}
            setStructureColour = {this.setStructureColour}
            setFixtureColour = {this.setFixtureColour}
            selectedFixtureStructure = {this.props.selectedFixtureStructure}
            setSelectedFixtureStructure = {this.props.setSelectedFixtureStructure}
            colour = {structure.colour}
            hubConnected = {this.props.hubConnected}
            selectedTool = {this.props.selectedTool}
            setTool = {this.props.setTool}
            onFixturePlace = {this.props.onFixturePlace}
          />
        )
      })}
    </Layer>);
  }

  setStructureColour = (id, colour) => {
    this.props.setStructureColour(this.props.data.id, id, colour);
  }

  setFixtureColour = (structureId, fixtureId, colour) => {
    this.props.setFixtureColour(this.props.data.id, structureId, fixtureId, colour);
  }
}

export class Structure extends Component {
  constructor(props) {
    super(props);

    this.state = {
      startPos: {x: 0, y: 0},
      singlePoint: this.props.points !== null && this.props.points.length === 1,
      newFixturePos: {x: 0, y: 0},
      newFixtureVisible: false
    }
  }

  render = () => {
    const points = this.props.points !== null && typeof this.props.points !== "undefined" ?
      [].concat.apply([], this.props.points.map(p => [p.x, p.y]))
      : [];

    const angle = this.props.points !== null && typeof this.props.points !== "undefined" ?
      DrawingUtils.lineAngle(this.props.points[1], this.props.points[0])
      : 0;

    let structure;
    const colour = typeof this.props.colour === "undefined" ? "#000" : this.props.colour;

    if (this.state.singlePoint) {
      structure = (
        <Circle
          key = {"c-" + this.props.id}
          x = {points[0]}
          y = {points[1]}
          fill = {colour}
          radius = {this.props.selected ? 0.3 : 0.2}
          strokeWidth = {0}
          hitStrokeWidth = {1}
          onMouseOver = {this.onMouseOver}
          onMouseOut = {this.onMouseOut}
          onMouseMove = {this.onMouseMove}
          onClick = {this.onClick}
        />
      )
    } else {
      structure = (
        <Line
          key = {"l-" + this.props.id}
          points = {points}
          stroke = {colour}
          strokeWidth = {this.props.selected ? 0.1 : 0.06}
          hitStrokeWidth = {1}
          onMouseOver = {this.onMouseOver}
          onMouseOut = {this.onMouseOut}
          onMouseMove = {this.onMouseMove}
          onClick = {this.onClick}
        />
      );
    }

    return (
      <Group
        draggable = {this.props.hubConnected}
        onDragStart = {this.handleDragStart}
        onDragMove = {this.handleDrag}
        onDragEnd = {this.handleDragEnd}
      >
        {structure}
        <Circle
          key = "fixture-preview"
          x = {this.state.newFixturePos.x}
          y = {this.state.newFixturePos.y}
          fill = {"#007bff"}
          radius = {0.2}
          visible = {this.state.newFixtureVisible && this.props.selectedTool === "add-fixture"}
          onClick = {this.onClick}
        />
        {this.props.fixtures.map(fixture => {
          return (
            <RiggedFixture
              key = {"rf-" + fixture.id}
              id = {fixture.id}
              position = {fixture.position}
              angle = {fixture.angle}
              structureId = {this.props.id}
              fixtureId = {fixture.fixture.id}
              selected = {this.props.selectedObjectId === fixture.id && this.props.selectedObjectType === "fixture"}
              onSelectObject = {this.props.onSelectObject}
              deselectObject = {this.props.deselectObject}
              hubConnected = {this.props.hubConnected}
              setFixtureColour = {this.setFixtureColour}
              selectedTool = {this.props.selectedTool}
              colour = {fixture.colour}
              setSelectedFixtureStructure = {this.props.setSelectedFixtureStructure}
              selectedFixtureStructure = {this.props.selectedFixtureStructure}
              setCursor = {this.props.setCursor}
              setHintText = {this.props.setHintText}
            />
          )
        })}
        <Text
          key={"sl-" + this.props.id}
          x = {points[0]}
          y = {points[1] - 0.1}
          rotation = {angle}
          padding = {2}
          text = {this.props.name}
          textScale = {0.05}
          fill = "#000"
        />
      </Group>
    );
  }

  handleDragStart = (event) => {
    this.setState({startPos: event.target.position()});
    this.props.setHintText("Release to confirm position");
  }

  handleDrag = (event) => {
    const pos = event.target.position();
    const change = DrawingUtils.getDifference(pos, this.state.startPos);
    const newPoint = {x: this.props.points[0].x + change.x, y: this.props.points[0].y + change.y};

    const snapPos = DrawingUtils.getNearestSnapPos(newPoint, this.props.snapGridSize);

    this.props.setTooltip({x: newPoint.x, y: newPoint.y + 25 / this.props.scale}, true, "(" + snapPos.x.toFixed(1) + "," + snapPos.y.toFixed(1) + ")");
  }

  handleDragEnd = (event) => {
    const snapPos = DrawingUtils.getNearestSnapPos(event.target.position(), this.props.snapGridSize);
    const change = DrawingUtils.getDifference(snapPos, this.state.startPos);

    const newStructurePoints = DrawingUtils.movePoints(this.props.points, change);
    const newFixtures = DrawingUtils.moveFixtures(this.props.fixtures, change);

    this.props.updatePoints(null, this.props.id, newStructurePoints, newFixtures);
    event.target.position({x: 0, y: 0});

    event.target.getLayer().draw();
    this.props.setTooltip({x: 0, y: 0}, false, "");
    this.props.setHintText("");
  }

  onMouseOver = (event) => {
    if (this.state.singlePoint) {
      event.target.radius(0.3);
    } else {
      event.target.strokeWidth(0.1);
    }

    event.target.draw();
    this.props.setCursor("pointer");
    if (this.props.selectedTool === "none") {
      this.props.setHintText("Click to select structure.\nClick and hold to move structure.")
    } else if (this.props.selectedTool === "add-fixture") {
      const stage = event.target.getStage();
      this.setState({
        newFixtureVisible: true,
        newFixturePos: DrawingUtils.nearestLinePoint(this.props.points, DrawingUtils.getRelativePointerPos(stage))
      }, () => {
        const snapPos = DrawingUtils.getNearestSnapPos(this.state.newFixturePos, this.props.snapGridSize);
        this.props.setTooltip({x: snapPos.x + 0.5, y: snapPos.y + 0.5}, true, "(" + snapPos.x.toFixed(1) + "," + snapPos.y.toFixed(1) + ")");
      });
    } else if (this.props.selectedTool === "eraser") {
      this.props.setHintText("Click to remove structure.")
    }
  }

  onMouseOut = (event) => {
    if (!this.props.selected) {
      if (this.state.singlePoint) {
        event.target.radius(0.2);
      } else {
        event.target.strokeWidth(0.06);
      }

      // prevent exception when object deleted whilst hovering on
      try {
        event.target.draw();
      } catch {}
      this.props.setHintText("");
    }

    if (this.props.selectedTool === "add-fixture") {
      this.setState({newFixtureVisible: false});
      this.props.setTooltip({x: 0, y: 0}, false, "");
    } else {
      this.props.setCursor("grab");
    }

  }

  onMouseMove = (event) => {
    if (this.props.selectedTool === "add-fixture") {
      const stage = event.target.getStage();
      this.setState({
        newFixturePos: DrawingUtils.nearestLinePoint(this.props.points, DrawingUtils.getRelativePointerPos(stage))
      }, () => {
        const snapPos = DrawingUtils.getNearestSnapPos(this.state.newFixturePos, this.props.snapGridSize);
        this.props.setTooltip({x: snapPos.x + 0.5, y: snapPos.y + 0.5}, true, "(" + snapPos.x.toFixed(1) + "," + snapPos.y.toFixed(1) + ")");
      });
    }
  }

  onClick = (event) => {
    if (this.props.hubConnected) {
      event.cancelBubble = true;
      this.props.deselectObject(this.props.selectedFixtureStructure);
      this.props.onSelectObject("structure", this.props.id, "");

      if (this.props.selectedTool === "eraser") {
        this.props.onStructureDelete();
        this.props.setTool("none");
      } else if (this.props.selectedTool === "add-fixture") {
        const stage = event.target.getStage();
        this.props.onFixturePlace(this.props.id, DrawingUtils.getNearestSnapPos(DrawingUtils.nearestLinePoint(this.props.points, DrawingUtils.getRelativePointerPos(stage)), this.props.snapGridSize));
        this.props.setTool("none");
        this.props.setTooltip({x: 0, y: 0}, false, "");
        this.setState({newFixtureVisible: false});
      } else if (this.props.selectedTool === "none") {
        this.props.setStructureColour(this.props.id, "#007bff");
        this.props.setHintText("Modify structure properties above.\nPress delete to remove structure.")
      }
    }
  }

  setFixtureColour = (id, colour) => {
    this.props.setFixtureColour(this.props.id, id, colour);
  }
}

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
    }
  }

  render = () => {
    const colour = typeof this.props.colour === "undefined" ? "#000" : this.props.colour;

    let highlight;
    if (colour !== "#000") {
      highlight = (
        <Circle
          x = {this.props.position.x}
          y = {this.props.position.y}
          width = {1.75}
          height = {1.75}
          fill = {colour}
          opacity = {.5}
        />
      );
    }

    return (
      <Group>
        {highlight}
        <Image
          x = {this.props.position.x}
          y = {this.props.position.y}
          offset = {{x: this.state.symbolWidth/2, y: this.state.symbolHeight/2}}
          scaleY = {-1}
          width = {this.state.symbolWidth}
          height = {this.state.symbolHeight}
          image = {this.state.symbol}
          onMouseEnter = {this.handleMouseEnter}
          onMouseLeave = {this.handleMouseLeave}
          onClick = {this.handleClick}
          rotation = {360-this.props.angle}
        />
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
      this.props.onSelectObject("fixture", this.props.structureId, this.props.id);

      if (this.props.selectedTool === "none") {
        this.props.setFixtureColour(this.props.id, "#007bff");
        this.props.setHintText("Modify fixture properties above.\nPress delete to remove fixture.")
      }
    }
  }
}

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

export class Tooltip extends Component {
  render = () => {
    return (
      <Label
        position = {this.props.position}
        scale = {{x: this.props.scale, y: -this.props.scale}}
        visible = {this.props.visible}
      >
        <Tag fill="#ddd"/>
        <KonvaText
          key = "tooltip"
          text = {this.props.text}
          fill = "#000"
          padding = {2}
        />
      </Label>
    )
  }
}