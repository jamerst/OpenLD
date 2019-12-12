import React, { Component, Fragment } from 'react';
import authService from './api-authorization/AuthorizeService';
import { Collapse,
  Col, Container, Row,
  Button, CustomInput, Form, FormGroup, Input, Label,
  Modal, ModalHeader, ModalBody, ModalFooter } from 'reactstrap';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';

import { FixtureResults } from './fixture-library/FixtureResults';
import { NewFixture } from './fixture-library/NewFixture';
import { SearchParams } from './fixture-library/SearchParams';

export class FixtureLibrary extends Component {
  static displayName = FixtureLibrary.name;

  constructor(props) {
    super(props);
    this.state = {
      results: [],
      types: [],
      searchParams: new SearchParams(),
      advancedSearch: false,
      addModalOpen: false,
      newFixture: new NewFixture(),
      loading: true,
      loadingTypes: true
    };

    this.fetchFixtures = this.fetchFixtures.bind(this);

    this.handleSearchChange = this.handleSearchChange.bind(this);
    this.handleSearchSubmit = this.handleSearchSubmit.bind(this);

    this.handleAddToggle = this.handleAddToggle.bind(this);
    this.handleAddChange = this.handleAddChange.bind(this);
    this.handleAddSubmit = this.handleAddSubmit.bind(this);
    this.handleImageChange = this.handleImageChange.bind(this);
  }

  componentDidMount() {
    const params = this.props.location.search;
    this.setState({ searchParams: new SearchParams(params) });
    document.title = "OpenLD Fixture Library";
    if (this.props.match.params.term) {
      this.setState({ searchTerm: this.props.match.params.term });
      this.fetchFixtures(this.props.match.params.term);
    } else {
      this.fetchFixtures("");
    }
    this.fetchFixtureTypes();
  }

  render() {
    const toggle = () => this.setState({ advancedSearch: !this.state.advancedSearch });

    let contents = this.state.loading
      ? <p className="text-center"><em>Loading results..</em></p>
      : FixtureResults.renderResults(this.state.results, this.handleClick);

    let types = this.renderTypes();

    return (
      <Fragment>
        <Container>
          <Row className="justify-content-between align-items-center">
            <Col>
              <h1>Fixture Library</h1>
            </Col>

            <Col xs="auto">
              <Button color="secondary" onClick={this.handleAddToggle}>Add Fixture</Button>
            </Col>
          </Row>
        </Container>

        <Form onSubmit={this.handleSearchSubmit} onChange={this.handleSearchChange} >
          <FormGroup row>
            <Col>
              <Input type="text" value={this.state.searchParams.name} name="name" placeholder="Name" />
            </Col>

            <Col xs="auto">
              <Button type="submit" color="primary">Search</Button>
            </Col>
          </FormGroup>

          <Button color="link" size="sm" onClick={toggle}>More Options <FontAwesomeIcon icon="chevron-down" /></Button>

          <Collapse isOpen={this.state.advancedSearch}>
            <FormGroup row>
              <Col xs="6">
                <Input type="text" size="sm" value={this.state.searchParams.manufacturer} name="manufacturer" placeholder="Manufacturer" />
              </Col>

              <Label for="searchType" size="sm">Fixture Type</Label>
              <Col>
                <CustomInput type="select" name="type" id="searchType" bsSize="sm" value={this.state.searchParams.type} name="type">
                  <option value="">Any</option>
                  {types}
                </CustomInput>
              </Col>
            </FormGroup>
          </Collapse>
        </Form>

        {contents}
        <p className="text-center"><em>Fixture information is not verified and may contain inaccuracies. No responsibility taken for data.</em></p>
        <Modal isOpen={this.state.addModalOpen}>
          <ModalHeader>Add New Fixture</ModalHeader>
          <Form onSubmit={this.handleAddSubmit} onChange={this.handleAddChange}>
            <ModalBody>
              <FormGroup>
                <Input type="text" value={this.state.newFixture.name} name="name" placeholder="Name"/>
              </FormGroup>

              <FormGroup>
                <Input type="text" value={this.state.newFixture.manufacturer} name="manufacturer" placeholder="Manufacturer"/>
              </FormGroup>

              <FormGroup>
                <Label for="addType">Fixture Type</Label>
                <CustomInput type="select" name="type" id="addType" value={this.state.newFixture.type.id} name="type">
                    {types}
                  </CustomInput>
              </FormGroup>

              <FormGroup row>
                <Col xs="6">
                  <Label for="addPower">Power (W)</Label>
                  <Input type="number" id="addPower" value={this.state.newFixture.power} name="power" min="0"/>
                </Col>
                <Col xs="6">
                  <Label for="addWeight">Weight (kg)</Label>
                  <Input type="number" id="addWeight" value={this.state.newFixture.weight} name="weight" step="0.1" min="0"/>
                </Col>
              </FormGroup>

              <FormGroup>
                <Label for="addImage">Fixture Image</Label>
                <Input type="file" id="addImage" name="image" onChange={this.handleImageChange}/>
              </FormGroup>
            </ModalBody>
            <ModalFooter>
              <Button type="submit"color="primary">Add</Button>
              <Button color="secondary" onClick={this.handleAddToggle}>Cancel</Button>
            </ModalFooter>
          </Form>
        </Modal>
      </Fragment>

    );
  }

  handleClick(id) {
    console.log(id);
  }

  handleSearchChange(event) {
    let params = this.state.searchParams;
    params[event.target.name] = event.target.value;
    this.setState({ searchParams: params });
  }

  handleSearchSubmit(event) {
    event.preventDefault();
    this.fetchFixtures(this.state.searchTerm);
    this.props.history.push("/library" + this.state.searchParams.getQueryString());
  }

  handleAddChange(event) {
    if (event.target.name !== "image") {
      let values = this.state.newFixture;

      if (event.target.name === "type") {
        values.type.id = event.target.value;
      } else {
        values[event.target.name] = event.target.value;
      }
      this.setState({ newFixture: values });
    }
  }

  handleAddToggle() {
    this.setState({ addModalOpen: !this.state.addModalOpen });
  }

  async handleImageChange(event) {
    let formData = new FormData();
    formData.append("file", event.target.files[0]);

    const response = await fetch('api/library/UploadFixtureImage', {
      method: "POST",
      headers: authService.generateAuthHeader(),
      body: formData
    });

    const data = await response.json();
    if (data.success) {
      let fixtureData = this.state.newFixture;
      fixtureData.image.id = data.data;
      this.setState({ newFixture: fixtureData });
    } else {
      alert("ERROR: " + data.msg);
    }
  }

  async handleAddSubmit(event) {
    event.preventDefault();
    const response = await fetch('api/library/CreateFixture', {
      method: "POST",
      headers: { ...authService.generateAuthHeader(), ...{ 'Content-Type': 'application/json' } },
      body: JSON.stringify(this.state.newFixture)
    });

    const data = await response.json();
    if (data.success === true) {
      this.setState({ addModalOpen: false, newFixture: new NewFixture() });
      this.fetchFixtures();
    } else {
      alert("ERROR: " + data.msg);
    }
  }

  async fetchFixtures() {
    const response = await fetch('api/library/GetFixtures', {
      method: "POST",
      headers: { ...authService.generateAuthHeader(), ...{ 'Content-Type': 'application/json' } },
      body: JSON.stringify(this.state.searchParams)
    });

    const data = await response.json();
    this.setState({ results: data.data, loading: false });
  }

  renderTypes() {
    return (
      <Fragment>
        {this.state.types.map(type =>
          <option value={type.id}>{type.name}</option>
        )}
      </Fragment>
    );
  }

  async fetchFixtureTypes() {
    const response = await fetch('api/library/GetFixtureTypes', {
      method: "POST",
      headers: authService.generateAuthHeader()
    });

    const data = await response.json();
    this.setState({ types: data.data, loadingTypes: false });
  }
}
