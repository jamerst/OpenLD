import React, { Component } from "react";
import { Layer } from "react-konva";
import { Structure } from "./Structure";

export class View extends Component {
  constructor(props) {
    super(props);
    this.loadingImages = {};
  }

  render = () => {
    if (typeof(this.props.data) === "undefined" || !this.props.data.structures) {
      return null;
    }

    if (this.props.data.structures.length === 0) {
      this.props.onLoad(this.props.data.id);
    }

    return (<Layer>
      {this.props.data.structures.map(structure => {
        this.loadingImages[structure.id] = false;

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
            onLoad = {this.onLoad}
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

  onLoad = (id) => {
    this.loadingImages[id] = true;

    let finished = true;
    Object.keys(this.loadingImages).forEach(key => {
      if (this.loadingImages[key] === false) {
        finished = false;
      }
    });

    if (finished === true) {
      this.props.onLoad(this.props.data.id);
    }
  }
}