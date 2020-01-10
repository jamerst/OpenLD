import React, { Component } from 'react';
import ReactKonvaCore from 'react-konva'

// Customised konva node components

export class Text extends Component {
    render = () => {
      return (
        <ReactKonvaCore.Text
          {...this.props}
          scale = {{x: this.props.textScale, y: -this.props.textScale}}
        />
      );
    }
  }