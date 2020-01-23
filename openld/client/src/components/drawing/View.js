import React, { Component } from "react";
import { Layer } from "react-konva";
import { Structure } from "./Structure";

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