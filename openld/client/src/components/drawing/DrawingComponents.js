import React, { Component } from "react";
import { Layer, Line } from "react-konva";

export class View extends Component {
  render() {
    console.log(this.props);
    return (<Layer>
      {this.props.data.structures.map(structure => {
        return (
          <Structure
            key={"s-" + structure.id}
            id={structure.id}
            points={structure.geometry.points}
            onDragMove = {this.props.onDragMove}
            onDragEnd = {this.props.onDragEnd}
          />
        )
      })}
    </Layer>);
  }
}

export class Structure extends Component {
  render() {
    let points = typeof this.props.points !== "undefined" ?
    [].concat.apply([], this.props.points.map(p => [p.x, p.y]))
    : [];

    return (
      <Line
        key={"l-" + this.props.id}
        points={points}
        stroke = "#000"
        strokeWidth = {0.05}
        draggable
        onDragMove = {this.props.onDragMove}
        onDragEnd = {this.props.onDragEnd}
      />
    );
  }
}