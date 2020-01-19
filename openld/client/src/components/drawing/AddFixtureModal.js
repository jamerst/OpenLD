import React, { Component } from "react";
import { Modal, ModalHeader, ModalBody } from 'reactstrap';
import { FixtureLibrary } from "../FixtureLibrary";

export class AddFixtureModal extends Component {
  render = () => {
    return (
      <Modal isOpen={this.props.isOpen} toggle={this.props.toggle} size="xl" centered>
        <ModalHeader toggle={this.props.toggle}>Add Fixture</ModalHeader>
        <ModalBody>
          Choose a fixture to add
          <FixtureLibrary
            embedded={true}
            xs="12" md="3"
            cardImgSize="15rem"
            height="50rem"
            onCardClick = {this.handleClick}
          />
        </ModalBody>
      </Modal>
    )
  }

  handleClick = (id) => {
    this.props.onChoose(id);
    this.props.toggle();
  }
}