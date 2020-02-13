import React, { Component } from "react";
import { Layer } from "react-konva";
import { Structure } from "./Structure";

export class View extends Component {
  constructor(props) {
    super(props);

    this.state = {
      symbolsLoaded: {}
    }
    this.symbolsLoaded = {};
  }

  static getDerivedStateFromProps = (nextProps) => {
    let symbols = {};
    nextProps.data.structures.forEach(structure => {
      symbols[structure.id] = false;
    });

    return {
      symbolsLoaded: symbols
    }
  }

  componentDidMount = () => {
    // trigger onSymbolLoad once rendered if no structures actually rendered
    if (this.props.data.structures.length === 0) {
      this.props.onSymbolLoad(this.props.data.id);
    }

    this.symbolsLoaded = this.state.symbolsLoaded;
  }

  render = () => {
    if (typeof(this.props.data) === "undefined" || !this.props.data.structures) {
      return null;
    }

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
            viewDimension = {this.props.data.width >= this.props.data.height ? this.props.data.width : this.props.data.height}
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