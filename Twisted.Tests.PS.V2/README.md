
# Twisted

An attempt to reverse-engineer the first games of the Twisted Metal franchise.

## Usage

### Supported games

- PC
  - Twisted Metal 1 Japanese release
- PSX
  - Twisted Metal 1 (SCES-00061)
  - Twisted Metal 1 (SCUS-94304)
  - Twisted Metal 1 (SIPS-60007)
  - Twisted Metal 2 (SCES-00567)
  - Twisted Metal 2 (SCUS-94306)
  - Twisted Metal 2 (SIPS-600021)

### Unit tests

The T4 template `TwistedTests\Generated.tt` generates one test per file found in `.twisted` directory.

You can setup your `.twisted` directory structure like the following or so:

- `.twisted`
  - `TM1PCJAP`
  - `TM1PSEUR`
  - `TM1PSJAP`
  - `TM1PSUSA`
  - `TM2PSEUR`
  - `TM2PSJAP`
  - `TM2PSUSA`