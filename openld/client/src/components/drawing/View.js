import React, { Component } from "react";
import { Layer } from "react-konva";
import { Structure } from "./Structure";
import { DrawingLabel } from "./DrawingLabel";

export class View extends Component {
  constructor(props) {
    super(props);

    this.symbolsLoaded = {};
  }

  UNSAFE_componentWillMount = () => {
    this.props.data.structures.forEach(structure => {
      this.symbolsLoaded[structure.id] = false;
    })
  }

  componentDidMount = () => {
    // trigger onSymbolLoad once rendered if no structures actually rendered
    if (this.props.data.structures.length === 0) {
      this.props.onSymbolLoad(this.props.data.id);
    }
  }

  render = () => {
    if (typeof(this.props.data) === "undefined" || !this.props.data.structures) {
      return null;
    }

    const viewDimension = this.props.data.width >= this.props.data.height ? this.props.data.width : this.props.data.height;

    return (<Layer>
      {this.props.data.structures.map(structure => {
        return (
          <Structure
            key={`s-${structure.id}`}
            id={structure.id}
            points={structure.geometry.points}
            fixtures={structure.fixtures}
            name={structure.name}
            snapGridSize = {this.props.snapGridSize}
            setTooltip = {this.props.setTooltip}
            setHintText = {this.props.setHintText}
            onMoveStructure = {this.props.onMoveStructure}
            onMoveFixture = {this.props.onMoveFixture}
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
            onSymbolLoad = {this.onSymbolLoad}
            viewDimension = {viewDimension}
          />
        )
      })}
      {this.props.data.labels.map(label => {
        return (
          <DrawingLabel
            key={`dl-${label.id}`}
            data = {label}
            selected = {this.props.selectedObjectId === label.id && this.props.selectedObjectType === "label"}
            selectedFixtureStructure = {this.props.selectedFixtureStructure}
            viewDimension = {viewDimension}
            hubConnected = {this.props.hubConnected}
            onSelectObject = {this.props.onSelectObject}
            deselectObject = {this.props.deselectObject}
            setLabelColour = {this.setLabelColour}
            setHintText = {this.props.setHintText}
            setTooltip = {this.props.setTooltip}
            selectedTool = {this.props.selectedTool}
            onMoveLabel = {this.props.onMoveLabel}
            setCursor = {this.props.setCursor}
            snapGridSize = {this.props.snapGridSize}
            scale = {this.props.scale}
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

  setLabelColour = (id, colour) => {
    this.props.setLabelColour(this.props.data.id, id, colour);
  }

  onSymbolLoad = (id) => {
    this.symbolsLoaded[id] = true;

    let finished = true;
    Object.keys(this.symbolsLoaded).forEach(key => {
      if (this.symbolsLoaded[key] === false) {
        finished = false;
      }
    });

    if (finished === true) {
      this.props.onSymbolLoad(this.props.data.id);
    }
  }
}