import React, { Component } from "react";
import { Circle, Line, Group } from "react-konva";
import { Text } from "./KonvaNodes";
import { RiggedFixture } from "./RiggedFixture";
import { DrawingUtils } from "./DrawingUtils";

export class Structure extends Component {
  constructor(props) {
    super(props);

    this.state = {
      startPos: {x: 0, y: 0},
      singlePoint: this.props.points !== null && this.props.points.length === 1,
      newFixturePos: {x: 0, y: 0},
      newFixtureVisible: false
    }

    this.loadingImages = {};
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

    if (this.props.fixtures.length === 0) {
      this.props.onLoad(this.props.id);
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
          this.loadingImages[fixture.id] = false;

          return (
            <RiggedFixture
              key = {"rf-" + fixture.id}
              data = {fixture}
              structureId = {this.props.id}
              fixtureId = {fixture.fixture.id}
              selected = {this.props.selectedObjectId === fixture.id && this.props.selectedObjectType === "fixture"}
              onSelectObject = {this.props.onSelectObject}
              deselectObject = {this.props.deselectObject}
              hubConnected = {this.props.hubConnected}
              setFixtureColour = {this.setFixtureColour}
              selectedTool = {this.props.selectedTool}
              setSelectedFixtureStructure = {this.props.setSelectedFixtureStructure}
              selectedFixtureStructure = {this.props.selectedFixtureStructure}
              setCursor = {this.props.setCursor}
              setHintText = {this.props.setHintText}
              onLoad = {this.onLoad}
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

  onLoad = (id) => {
    this.loadingImages[id] = true;

    let finished = true;
    Object.keys(this.loadingImages).forEach(key => {
      if (this.loadingImages[key] === false) {
        finished = false;
      }
    });

    if (finished === true) {
      this.props.onLoad(this.props.id);
    }
  }
}