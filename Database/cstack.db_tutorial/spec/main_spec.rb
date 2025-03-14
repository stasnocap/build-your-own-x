describe 'database' do
  before do
    `rm -rf test.db`
  end

  def run_script(commands)
    raw_output = nil
    IO.popen("./main test.db", "r+") do |pipe|
      commands.each do |command|
        pipe.puts command
      end

      pipe.close_write

      # Read entire output
      raw_output = pipe.gets(nil)
    end
    raw_output.split("\n")
  end

  it 'inserts and retrieves a row' do
    result = run_script([
      "insert 1 user1 person1@example.com",
      "select",
      ".exit",
    ])
    expect(result).to match_array([
      "db > Executed.",
      "db > (1, user1, person1@example.com)",
      "Executed.",
      "db > ",
    ])
  end

  it 'prints error message when table is full' do
    script = (1..1301).map do |i|
      "insert #{i} user#{i} person#{i}@example.com"
    end
    script << ".exit"
    result = run_script(script)
    expect(result.last(2)).to match_array([
      "db > Error: Table full.",
      "db > ",
    ])
  end

  it 'allows inserting strings that are the maximum length' do
    username = "a" * 32
    email = "a" * 255
    script = [
      "insert 1 #{username} #{email}",
      "select",
      ".exit"
    ]
    result = run_script(script)
    expect(result).to match_array([
      "db > Executed.",
      "db > (1, #{username}, #{email})",
      "Executed.",
      "db > ",
    ])
  end

  it 'prints error message if strings too long' do
    username = "a" * 33
    email = "a" * 255
    script = [
      "insert 1 #{username} #{email}",
      "select",
      ".exit"
    ]
    result = run_script(script)
    expect(result).to match_array([
      "db > String is too long.",
      "db > Executed.",
      "db > "
    ])
  end

  it 'prints an error message if id is negative' do
    script = [
      "insert -1 cstack foo@bar.com",
      "select",
      ".exit"
    ]
    result = run_script(script)
    expect(result).to match_array([
      "db > ID must be positive.",
      "db > Executed.",
      "db > ",
    ])
  end

  it 'keeps data after closing connection' do
    result1 = run_script([
      "insert 1 user1 person1@example.com",
      ".exit",
    ])
    expect(result1).to match_array([
      "db > Executed.",
      "db > ",
    ])

    result2 = run_script([
      "select",
      ".exit",
    ])
    expect(result2).to match_array([
      "db > (1, user1, person1@example.com)",
      "Executed.",
      "db > ",
    ])
  end
end