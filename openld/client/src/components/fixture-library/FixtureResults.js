import React, { Component, Fragment } from 'react';
import { Card, CardImg, CardBody, CardTitle, Col, Container, Row } from 'reactstrap';

export class FixtureResults extends Component {
  render = () => {
    if (this.props.results.length > 0) {
      return (
        <Fragment>
          <Container>
            <Row className="justify-content-around">
            {this.props.results.map(result =>
              <Col xs="12" md="4" key={result.id}>
                <Card color="light" onClick={() => this.props.onCardClick(result.id)} className="mb-3">
                  <div className="p-3" style={{ width: "100%", textAlign: "center", backgroundColor: "white" }}>
                    <CardImg src={ "/api/fixture/GetImage/" + result.id } style={{ maxHeight: "30rem", width: "auto", maxWidth: "100%" }}></CardImg>
                  </div>
                  <CardBody>
                    <CardTitle className="h3">{result.name}</CardTitle>
                      <dl>
                        <dd>{result.manufacturer}</dd>
                        <dd>{result.type.name}</dd>

                        <dt>Power</dt>
                        <dd>{result.power}W</dd>
                      </dl>
                  </CardBody>
                </Card>
              </Col>
            )}
            </Row>
          </Container>
          <p className="text-center"><em>Fixture information is not verified and may contain inaccuracies. No responsibility taken for data.</em></p>
        </Fragment>
      );
    } else {
      return (
        <p className="text-center"><em>No results found</em></p>
      )
    }
  }
}